using Dapper;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MimeMapping;
using Sistema_Legal_2._0.Server.Entities;
using Sistema_Legal_2._0.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Sistema_Legal_2._0.Server.Infraestructure.Mailing;

namespace Sistema_Legal_2._0.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {

        private readonly string _filesServerPath;
        private readonly string _cadenaSQL;
        private IConfiguration  configuration;
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

/// <summary>
/// Controlador encardado de subir evidencias en la ultima audiencia creada esto se hace validando por la fecha se subira en el litigio
/// que tenga la fecha mas reciente este controlador espera que ya exista una audienca para poder crear una evidencia en la tabla evidencia en la base de datos 
/// </summary>
/// <param name="model"></param>
/// <returns></returns>

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
        /// <summary>
        /// Este controlador se encaga de crear audiencas extras a un litigio (El litigio de forma automatica crea la primera audiencia) 
        /// espera el parametro del idlitigio ademas de eso tiene que tener una fecha superior a la ultima audiencia que tiene un litigio
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

        [HttpPost("crearAudiencias")]
        public async Task<IActionResult> CrearAudiencia([FromBody] CrearAudienciaDto dto)
        {
            try
            {
                if (dto.Fecha == null)
                    return BadRequest(new { success = false, error = "La fecha no puede ser nula." });

                var fechaLocal = dto.Fecha.Value.ToLocalTime();

                using var conn = new SqlConnection(_cadenaSQL);
                await conn.OpenAsync();

                var idAudiencia = await conn.QueryFirstAsync<int>(
                    "InsertarAudiencia",
                    new
                    {
                        IdLitigio = dto.IdLitigio,
                        Numero = dto.Numero,
                        Tipo = dto.Tipo,
                        Fecha = fechaLocal,
                        SalaId = dto.SalaId,
                        id_usuario = dto.id_usuario,
                        Cierre = dto.Cierre
                    },
                    commandType: CommandType.StoredProcedure
                );

                 await CorreoHelper.EnviarCorreoAudiencia(idAudiencia, conn);

                return Ok(new
                {
                    success = true,
                    message = "Audiencia creada correctamente",
                    idAudiencia
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
        /// <summary>
        /// Este controlador se encarga de crear un estado permitiendo ademas cambiar el estado del litigio para pasar los las diferentes fases
        /// se debe de dar un idlitigio ademas de que la fecha tiene que ser mas alta que el ultimo litigio registrado
        /// en caso de que el campo cierre sea true esto hara que el litigio se considere como finalizado y ya no se pueda agregar mas documentacion relacionada
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

        [HttpPost("CrearEstados")]
        public async Task<IActionResult> CrearEstados([FromBody] CrearEstados dto)
        {
            try
            {
                if (dto.Fecha == null)
                    return BadRequest(new { success = false, error = "La fecha no puede ser nula." });

                var fechaLocal = dto.Fecha.Value.ToLocalTime();

                using var conn = new SqlConnection(_cadenaSQL);
                await conn.OpenAsync();

                var idAudiencia = await conn.QueryFirstAsync<int>(
                    "InsertarEstado",
                    new
                    {
                        IdLitigio = dto.IdLitigio,
                        Numero = dto.Numero,
                        Tipo = dto.Tipo,
                        Fecha = fechaLocal,
                        SalaId = dto.SalaId,
                        id_usuario = dto.id_usuario,
                        Cierre = dto.Cierre,
                        NuevoEstadoLitigio = dto.NuevoEstadoLitigio
                    },
                    commandType: CommandType.StoredProcedure
                );

               await CorreoHelper.EnviarCorreoAudiencia(idAudiencia, conn);

                return Ok(new
                {
                    success = true,
                    message = "Estado registrado y audiencia creada con éxito.",
                    idAudiencia
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

        /// <summary>
        /// En caso de una mala digitacion en cuanto a las audiencas y estados este controlador es para corregir esos errores de sintaxis no opstante
        /// en el caso de los estados no permite cambiar el estado una vez ya definido este solo editara la ultima audiencia creada osea audiencias previas 
        /// se consideran como finalizadas, espera un idlitigio y que la fecha sea mas reciente que el litigio pasado
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

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

               await CorreoHelper.EnviarCorreoAudienciaActualizada(dto.IdLitigio, connection);

                return Ok(new
                {
                    success = true,
                    message = "Última audiencia actualizada correctamente y correo enviado"
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

        /// <summary>
        /// Este controlador nos permite acceder a la informacion que posee la ultima audiencia
        /// esto nos permite cargar la informacion previa como la sala y las fechas para que sea mas facil 
        /// llenar la informacion
        /// </summary>
        /// <param name="idLitigio"></param>
        /// <returns></returns>


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

        /// <summary>
        /// Funcion que nos permite normalizar la extension del archivo 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Este controlador nos unifica la ruta que tenemos guardada en la base de datos (La cual esta por la mitad) y la ruta que tenemos en el servidor (La otra mitad) 
        /// unificandolas para poder acceder al archivo 
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>

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
                return File(fileStream, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al acceder al archivo", error = ex.Message });
            }
        }

        /// <summary>
        /// Esto nos trae todas las audiencias que han sido agregadas al sistema para mostrarlas en la vista calendario (Administrador)
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Controlador que nos permite ver todas las audiencias que tiene un abogado esperamos el idUsuario y retornamos solo las de ese usuario en especifico (Abogado)
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpGet("AudienciasCalendarios/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<AudienciaCalendarioDtoAbogados>>> GetAudienciasCalendarioParaAbogados(int idUsuario)
        {
            var result = new List<AudienciaCalendarioDtoAbogados>();

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal")))
            {


                using (SqlCommand cmd = new SqlCommand("sp_GetAudienciasPorUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                   
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new AudienciaCalendarioDtoAbogados
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









