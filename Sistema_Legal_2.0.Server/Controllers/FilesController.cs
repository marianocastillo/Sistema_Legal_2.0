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

                // ✅ Obtener ID del usuario desde Claims (o donde lo guardes)
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // o "sub", o tu propio claim
                int? usuarioId = usuarioIdClaim != null ? int.Parse(usuarioIdClaim.Value) : null;

                // ✅ Guardar el usuario en SQL Server Session Context
                if (usuarioId.HasValue)
                {
                    await conn.ExecuteAsync("EXEC sp_set_session_context @key, @value", new
                    {
                        key = "usuario_id",
                        value = usuarioId.Value
                    });
                }

                // 🟢 Ejecutar el SP normalmente
                var result = await conn.QueryFirstAsync<int>(
                    "InsertarAudiencia",
                    new
                    {
                        IdLitigio = dto.IdLitigio,
                        Numero = dto.Numero,
                        Tipo = dto.Tipo,
                        Fecha = dto.Fecha,
                        SalaId = dto.SalaId
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

                // ✅ Establecer usuario_id en session_context si aplica
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // o "sub", o personalizado
                int? usuarioId = usuarioIdClaim != null ? int.Parse(usuarioIdClaim.Value) : null;

                if (usuarioId.HasValue)
                {
                    await connection.ExecuteAsync("EXEC sp_set_session_context @key, @value", new
                    {
                        key = "usuario_id",
                        value = usuarioId.Value
                    });
                }

                // 🟢 Ejecutar SP con la sala en lugar del tribunal
                var parametros = new
                {
                    IdLitigio = dto.IdLitigio,
                    Numero = dto.Numero,
                    Tipo = dto.Tipo,
                    Fecha = fechaLocal,
                    SalaId = dto.SalaId
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
    }
}








