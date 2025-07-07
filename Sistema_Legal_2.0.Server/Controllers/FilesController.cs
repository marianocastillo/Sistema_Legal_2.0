using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExcelDataReader;
using System.Data;
using System.Reflection;
using System.Numerics;
using Dapper;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using Sistema_Legal_2._0.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Entities;
using System.Configuration;
using Microsoft.Data.SqlClient;
using MimeMapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Security.Claims;

namespace Sistema_Legal_2._0.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {

        private readonly string _filesServerPath;
        private readonly string _cadenaSQL;
        private IConfiguration configuration;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;
        private readonly db_silegContext _db_silegContext;
        public FilesController(IConfiguration config, db_silegContext db_SilegContext)
        {
            _db_silegContext = db_SilegContext;
            _filesServerPath = config.GetSection("Configuracion").GetSection("FilesServerPath").Value;
            _cadenaSQL = config.GetConnectionString("Sistema_Legal");
            _configuration = config;
        }

        [HttpPost("subir-evidencia")]
        public async Task<IActionResult> SubirEvidencia([FromForm] EvidenciaUploadModel model)
        {
            try
            {
                using var conn = new SqlConnection(_cadenaSQL);
                var resultado = await conn.QueryFirstAsync<dynamic>(
                       "InsertarEvidenciaEnUltimaAudiencia",
                       new
                       {
                           IdLitigio = model.IdLitigio,                         
                           IdUsuario = model.IdUsuario,
                           NombreEvidencia = model.NombreEvidencia,
                           NombreArchivo = model.Archivo.FileName,
                           Comentario = model.Comentario

                       },
       commandType: CommandType.StoredProcedure
   );
                string rutaBase = @"\\192.168.3.95\FileSharing\Archivos_Sileg";
                string carpetaDestino = Path.Combine(
     rutaBase,
     resultado.IdLitigio.ToString(),
     resultado.Acto.ToString(),
     resultado.NumeroAudiencia.ToString(),
     model.NombreEvidencia
 );


                if (!Directory.Exists(carpetaDestino))
                    Directory.CreateDirectory(carpetaDestino);

                string rutaCompleta = Path.Combine(carpetaDestino, resultado.NombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await model.Archivo.CopyToAsync(stream);
                }

                return Ok(new
                {
                    success = true,
                    ruta = Path.Combine(
                        resultado.IdLitigio.ToString(),
                        resultado.NumeroAudiencia.ToString(),
                        model.NombreEvidencia,
                        resultado.NombreArchivo
                    )
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }


        [HttpPost("crearAudiencias")]
        public async Task<IActionResult> CrearAudiencia([FromBody] CrearAudienciaDto dto)
        {
            try
            {
                using var conn = new SqlConnection(_cadenaSQL);
                await conn.OpenAsync();

                var result = await conn.QueryFirstAsync<int>(
                    "InsertarAudiencia",
                    new
                    {
                        IdLitigio = dto.IdLitigio,
                        Numero = dto.Numero,
                        Tipo = dto.Tipo,
                        Fecha = dto.Fecha,
                        SalaId = dto.SalaId,
                        id_usuario  = dto.id_usuario
                    },
                    commandType: CommandType.StoredProcedure
                );

                return Ok(new
                {
                    success = true,
                    message = "Audiencia creada correctamente",
                    idAudiencia = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpPut("actualizarAudiencias")]
        public async Task<IActionResult> EditarUltimaAudiencia([FromBody] AudienciaUpdateDto dto)
        {
            try
            {
                var fechaLocal = dto.Fecha.ToLocalTime();

                using var connection = new SqlConnection(_cadenaSQL);
                await connection.OpenAsync();


       
                var parametros = new
                {
                    IdLitigio = dto.IdLitigio,
                    Numero = dto.Numero,
                    Tipo = dto.Tipo,
                    Fecha = fechaLocal,
                    SalaId = dto.SalaId,
                    id_usuario = dto.id_usuario
                };

                await connection.ExecuteAsync("ActualizarAudiencia", parametros, commandType: CommandType.StoredProcedure);

                return Ok(new
                {
                    success = true,
                    message = "Última audiencia actualizada correctamente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al actualizar la última audiencia",
                    error = ex.Message
                });
            }
        }


        [HttpGet("ultima-audiencia/{idLitigio}")]
        public async Task<IActionResult> ObtenerUltimaAudiencia(int idLitigio)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
                var resultado = await conn.QueryFirstOrDefaultAsync(new CommandDefinition(
                    "sp_ObtenerUltimaAudiencia",
                    new { IdLitigio = idLitigio },
                    commandType: CommandType.StoredProcedure
                ));

                if (resultado == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No se encontró ninguna audiencia para este litigio."
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener la última audiencia.",
                    error = ex.Message
                });
            }
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }


        [HttpGet("rutaspor/{*ruta}")]
        public IActionResult ObtenerArchivoPorRuta(string ruta)
        {
            try
            {
                string rutaBase = @"\\192.168.3.95\FileSharing\Archivos_Sileg";
                string rutaCompleta = Path.Combine(rutaBase, ruta);

                if (!System.IO.File.Exists(rutaCompleta))
                {
                    return NotFound(new { mensaje = "El archivo no existe." });
                }

                var contentType = GetContentType(rutaCompleta);
                var fileStream = new FileStream(rutaCompleta, FileMode.Open, FileAccess.Read);
                return File(fileStream, contentType); // 👈 No fuerza descarga
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al acceder al archivo", error = ex.Message });
            }
        }
        [HttpGet("AudienciasHistorial")]
        public async Task<ActionResult<IEnumerable<AudienciaCalendarioDto>>> GetAudienciasCalendario()
        {
            var result = new List<AudienciaCalendarioDto>();

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal")))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAudienciasCalendarioFull", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new AudienciaCalendarioDto
                            {
                                Id_audiencia = reader.GetInt32(reader.GetOrdinal("Id_audiencia")),
                                NumeroAudiencia = reader["NumeroAudiencia"]?.ToString(),
                                TipoAudiencia = reader["TipoAudiencia"]?.ToString(),
                                FechaAudiencia = reader.GetDateTime(reader.GetOrdinal("FechaAudiencia")),
                                IdSala = reader["IdSala"] as int?,
                                NombreSala = reader["NombreSala"]?.ToString(),
                                Id_Tribunal = reader["Id_Tribunal"] as int?,
                                Nombre_Tribunal = reader["Nombre_Tribunal"]?.ToString(),
                                id_Ltg = reader["id_Ltg"] as int?,
                                ltg_acto = reader["ltg_acto"]?.ToString(),
                                ltg_Nombre_Demandante = reader["ltg_Nombre_Demandante"]?.ToString(),
                                id_demanda = reader["id_demanda"] as int?,
                                TipoDemanda = reader["TipoDemanda"]?.ToString()
                            });
                        }
                    }
                }
            }

            return Ok(result);
        }
    }
}









