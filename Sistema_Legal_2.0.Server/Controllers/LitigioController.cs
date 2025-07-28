using Dapper;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MimeMapping;
using Newtonsoft.Json;
using Sistema_Legal_2._0.Server.Entities;
using Sistema_Legal_2._0.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Sistema_Legal_2._0.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LitigioController : ControllerBase
    {

        private readonly string _filesServerPath;
        private readonly string _cadenaSQL;
        private IConfiguration configuration;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;
        private readonly db_silegContext _db_silegContext;
        public LitigioController(IConfiguration config, db_silegContext db_SilegContext)
        {
            _db_silegContext = db_SilegContext;
            _filesServerPath = config.GetSection("Configuracion").GetSection("FilesServerPath").Value;
            _cadenaSQL = config.GetConnectionString("Sistema_Legal");
            _configuration = config;
        }

        /// <summary>
        /// Controlador que nos permite editar el litigio por posibles errores de sintaxis aqui le pasamos todos los parametros mediante otro controlador que los llena en el frontend
        /// </summary>
        /// <param name="litigio"></param>
        /// <returns></returns>
        [HttpPut("EditarLitigio")]
        public async Task<IActionResult> EditarLitigio([FromBody] LitigioEditarDto litigio)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(_cadenaSQL))
            {


                await connection.OpenAsync();

                litigio.ltg_acto = string.IsNullOrWhiteSpace(litigio.ltg_acto) ? null : litigio.ltg_acto;
                litigio.ltg_Cedula_Demandante = string.IsNullOrWhiteSpace(litigio.ltg_Cedula_Demandante) ? null : litigio.ltg_Cedula_Demandante;
                litigio.ltg_Nacionalidad = string.IsNullOrWhiteSpace(litigio.ltg_Nacionalidad) ? null : litigio.ltg_Nacionalidad;
                litigio.ltg_Tipo_Demandante = string.IsNullOrWhiteSpace(litigio.ltg_Tipo_Demandante) ? null : litigio.ltg_Tipo_Demandante;
                litigio.ltg_Cedula_Representante = string.IsNullOrWhiteSpace(litigio.ltg_Cedula_Representante) ? null : litigio.ltg_Cedula_Representante;
                litigio.ltg_Nombre_Representante = string.IsNullOrWhiteSpace(litigio.ltg_Nombre_Representante) ? null : litigio.ltg_Nombre_Representante;


                using (SqlCommand cmd = new SqlCommand("sp_EditarLitigio", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_Ltg", litigio.id_Ltg);
                    cmd.Parameters.AddWithValue("@ltg_acto", (object?)litigio.ltg_acto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Fecha_Acto", litigio.ltg_Fecha_Acto);
                    cmd.Parameters.AddWithValue("@ltg_Nombre_Demandante", (object?)litigio.ltg_Nombre_Demandante ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_Tipo_Demanda", litigio.id_Tipo_Demanda);
                    cmd.Parameters.AddWithValue("@ltg_Cedula_Demandante", (object?)litigio.ltg_Cedula_Demandante?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Nacionalidad", (object?)litigio.ltg_Nacionalidad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Nacionalidad_Representante", (object?)litigio.ltg_Nacionalidad_Representante ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Tipo_Demandante", litigio.ltg_Tipo_Demandante);
                    cmd.Parameters.AddWithValue("@ltg_Cedula_Representante", (object?)litigio.ltg_Cedula_Representante ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Nombre_Representante", (object?)litigio.ltg_Nombre_Representante ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_Sentencia", litigio.id_Sentencia);
                    cmd.Parameters.AddWithValue("@id_Estatus", litigio.id_Estatus);
                    cmd.Parameters.AddWithValue("@id_usuario", litigio.id_usuario);


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                        return NotFound("Litigio no encontrado o no modificado.");

                    return Ok(new { mensaje = "Litigio actualizado correctamente." });
                }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al actualizar el litigio",
                    error = ex.Message 
                });
            }


        }
        /// <summary>
        /// Controlador que nos permite la creacion de un nuevo litigio pero ademas de agregar datos en la tabla litigios tambien sube datos a Audiencias y Evidencias
        /// En caso de no tener la informacion de la audiencia se crea una automatico pero sin fecha ni sala donde se realizara 
        /// </summary>
        /// <param name="datos"></param>
        /// <returns></returns>

        [HttpPost("Subir_Litigio_Con_Archivo")]
        public async Task<IActionResult> SubirLitigioConArchivo([FromForm] LitigioConArchivo datos)
        {
            int idLitigio = 0;
            int idAudiencia = 0;
            string rutaRelativa = "";

            try
            {
                if (datos.Archivo == null || datos.Archivo.Length == 0)
                    return BadRequest("No se recibió ningún archivo.");

                string nombreArchivo = Path.GetFileName(datos.Archivo.FileName);
                string nombreCarpeta = Path.GetFileNameWithoutExtension(nombreArchivo);
                var fechabien = datos.ltg_Fecha_Audiencia?.ToLocalTime();

                // 1. Crear litigio y obtener ID
                using (var connection = new SqlConnection(_cadenaSQL))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_CrearLitigioCompleto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // ... parámetros del litigio ...
                        command.Parameters.AddWithValue("@ltg_acto", datos.ltg_acto);
                        command.Parameters.AddWithValue("@ltg_Fecha_Acto", datos.ltg_Fecha_Acto);
                        command.Parameters.AddWithValue("@ltg_Cedula_Demandante", datos.ltg_Cedula_Demandante);
                        command.Parameters.AddWithValue("@ltg_Nombre_Demandante", datos.ltg_Nombre_Demandante);
                        command.Parameters.AddWithValue("@ltg_Tipo_Demandante", datos.ltg_Tipo_Demandante);
                        command.Parameters.AddWithValue("@ltg_Nacionalidad", datos.ltg_Nacionalidad);
                        command.Parameters.AddWithValue("@ltg_Cedula_Representante", (object?)datos.ltg_Cedula_Representante ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ltg_Nombre_Representante", (object?)datos.ltg_Nombre_Representante ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ltg_Nacionalidad_Representante", datos.ltg_Nacionalidad_Representante);
                        command.Parameters.AddWithValue("@id_Tipo_Demanda", datos.id_Tipo_Demanda);
                        command.Parameters.AddWithValue("@id_Sentencia", (object?)datos.id_Sentencia ?? DBNull.Value);
                        command.Parameters.AddWithValue("@id_usuario", datos.id_usuario);
                        command.Parameters.AddWithValue("@id_Estatus", datos.id_Estatus);

                        command.Parameters.AddWithValue("@Nombre_Evidencia", string.IsNullOrWhiteSpace(datos.NombreEvidencia) ? nombreCarpeta : datos.NombreEvidencia);
                        command.Parameters.AddWithValue("@Comentario_Evidencia", string.IsNullOrWhiteSpace(datos.comentario) ? "Archivo subido sin nombre." : datos.comentario);

                        command.Parameters.AddWithValue("@Fecha", fechabien);
                        command.Parameters.AddWithValue("@SalaId", datos.SalaId);                   
                        command.Parameters.AddWithValue("@Tipo", (object?)datos.Tipo_audiencia ?? DBNull.Value);


                        // Ruta temporal (puedes enviar algo por ahora)
                        command.Parameters.AddWithValue("@Ruta_Archivo", "TEMP");
                        command.Parameters.AddWithValue("@Nombre_Archivo", nombreArchivo);

                        using var reader = await command.ExecuteReaderAsync();
                        if (await reader.ReadAsync())
                        {
                            idLitigio = reader.GetInt32(reader.GetOrdinal("id_litigio"));
                            idAudiencia = reader.GetInt32(reader.GetOrdinal("id_audiencia"));
                        }
                    }
                }

                // 2. Guardar archivo usando el idLitigio correcto
                string rutaBase = @"\\192.168.3.95\FileSharing\Archivos_Sileg";
                string rutaFinal = Path.Combine(rutaBase, idLitigio.ToString(), datos.ltg_acto, nombreCarpeta);
                Directory.CreateDirectory(rutaFinal);

                string rutaArchivoCompleta = Path.Combine(rutaFinal, nombreArchivo);
                using (var stream = new FileStream(rutaArchivoCompleta, FileMode.Create))
                {
                    await datos.Archivo.CopyToAsync(stream);
                }

                // 3. Actualizar ruta en base de datos si lo necesitas
                rutaRelativa = Path.Combine(idLitigio.ToString(), datos.ltg_acto, nombreCarpeta, nombreArchivo);

                // Luego de copiar el archivo
                using (var conn = new SqlConnection(_cadenaSQL))
                {
                    await conn.OpenAsync();

                    var updateCmd = new SqlCommand("UPDATE Evidencias SET Ruta_Archivo = @Ruta WHERE Id_Audiencia = @IdAudiencia", conn);
                    updateCmd.Parameters.AddWithValue("@Ruta", rutaRelativa);
                    updateCmd.Parameters.AddWithValue("@IdAudiencia", idAudiencia);
                    await updateCmd.ExecuteNonQueryAsync();
                }


                return Ok(new
                {
                    mensaje = "Litigio, audiencia y archivo creados correctamente.",
                    id_litigio = idLitigio,
                    id_audiencia = idAudiencia,
                    rutaRelativa
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Aqui llenamos la informacion de la ultima audiencia en Litigio Detallado donde nos trae las audiencias evidencias y el tribunale mediante el id sala
        /// </summary>
        /// <param name="id_litigio"></param>
        /// <returns></returns>

        [HttpGet("audiencias-con-evidencias-y-tribunal/{id_litigio}")]
        public async Task<IActionResult> ObtenerAudienciasYTribunal(int id_litigio)
{
    var result = new LitigioDetalleDto();

    try
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
        await connection.OpenAsync();

        using var command = new SqlCommand("sp_ObtenerAudienciasConEvidenciasYTribunalFinal", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@id_litigio", id_litigio);

        using var reader = await command.ExecuteReaderAsync();

        // 1. Audiencias con evidencias, sala y tribunal
        while (await reader.ReadAsync())
        {
            var audiencia = new AudienciaDto
            {
                Id_audiencia = reader["Id_audiencia"] as int?,
                numeroAudiencia = reader["numeroAudiencia"] as string,
                tipoAudiencia = reader["tipoAudiencia"] as string,
                fechaAudiencia = reader["fechaAudiencia"] as DateTime?,
                IdSala = reader["IdSala"] as int?,
                evidenciasYComentarios = reader["evidenciasJSON"] == DBNull.Value
                    ? new List<EvidenciaDto>()
                    : JsonConvert.DeserializeObject<List<EvidenciaDto>>(reader["evidenciasJSON"].ToString())
            };

            result.Audiencias.Add(audiencia);
        }

        if (await reader.NextResultAsync() && await reader.ReadAsync())
        {
            result.TribunalFinal = new AudienciaFinalDto
            {
                tipoAudiencia = reader["Tipo"] as string,
                numeroAudiencia = reader["Numero"] as string,
                IdSala = reader["IdSala"] as int?,
                Nombre = reader["Nombre"] as string,
                id_Tribunal = reader["Id_Tribunal"] as int?,
                Distrito = reader["Distrito"] as string,
                MapsUrl = reader["MapsUrl"] as string,
                nombre_Tribunal = reader["Nombre_Tribunal"] as string,            
                tribunal_Direccion = reader["tribunal_Direccion"] as string,
                tribunal_Telefono = reader["tribunal_Telefono"] as string,
                tribunal_Descripcion = reader["tribunal_Descripcion"] as string,
                ltg_Fecha_Audiencia = reader["ltg_Fecha_Audiencia"] as DateTime?
            };
        }

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            success = false,
            message = "Error al obtener los datos",
            error = ex.Message
        });
    }
}


        /// <summary>
        /// Controlador para traer la informacion mediante el RNC o la CEDULA nos usamos un procedimiento almacenado para eso
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>

        [HttpGet("BuscarDocumento/{documento}")]
        [AllowAnonymous]
        public IActionResult BuscarDocumento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return BadRequest("Documento vacío");

            var resultado = new
            {
                Nombre = "",
                Nacionalidad = ""
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal")))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Consulta_Documento", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Documento", documento);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Si es persona
                                if (reader.FieldCount == 3)
                                {
                                    resultado = new
                                    {
                                        Nombre = reader["Nombre_Completo"].ToString(),
                                        Nacionalidad = reader["Nacionalidad"].ToString()
                                    };
                                }
                                // Si es empresa
                                else if (reader.FieldCount == 2)
                                {
                                    resultado = new
                                    {
                                        Nombre = reader["Empresa"].ToString(),
                                        Nacionalidad = "DOMINICANA"
                                    };
                                }
                            }
                        }
                    }
                }

                return Ok(resultado);
            }
            catch (SqlException ex)
            {
                // Puedes loguear el error si usas logging
                Console.WriteLine("Error de SQL Server: " + ex.Message);

                return StatusCode(500, new
                {
                    Mensaje = "No se pudo conectar con la base de datos.",
                    Detalles = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Captura errores inesperados
                return StatusCode(500, new
                {
                    Mensaje = "Error inesperado al procesar la solicitud.",
                    Detalles = ex.Message
                });
            }
        }

        /// <summary>
        /// Controlador para los dropdowns se llenen de la informacion para que el usuario pueda elegir en el registro de litigios y la modificacion
        /// </summary>
        /// <returns></returns>
        /// 

        [HttpGet("datos-litigio")]
        public async Task<IActionResult> ObtenerDatosLitigio()
        {
            var datos = new DatosLitigioDto
            {
                Tribunales = new List<TribunalDto>(),
                TiposDemanda = new List<TipoDemandaDto>(),
                EstatusLitigios = new List<EstatusLitigioDto>(),
                Salas = new List<Salao>()
            };

            using (var connection = _db_silegContext.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_ObtenerDatosLitigio";
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                      
                        while (await reader.ReadAsync())
                        {
                            datos.Tribunales.Add(new TribunalDto
                            {
                                Id_Tribunal = reader.GetInt32(0),
                                Nombre_Tribunal = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Telefono = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Estatus = reader.IsDBNull(4) ? false : reader.GetBoolean(4),
                                Distrito = reader.IsDBNull(5) ? null : reader.GetString(5),
                                MapsUrl = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Direccion = reader.IsDBNull(7) ? null : reader.GetString(7)
                            });
                        }

                        await reader.NextResultAsync();
                        while (await reader.ReadAsync())
                        {
                            datos.TiposDemanda.Add(new TipoDemandaDto
                            {
                                id_demanda = reader.IsDBNull(1) ? null : reader.GetInt32(0),
                                Nombre = reader.IsDBNull(1) ? null : reader.GetString(1),
                            });
                        }

                   
                        await reader.NextResultAsync();
                        while (await reader.ReadAsync())
                        {
                            datos.EstatusLitigios.Add(new EstatusLitigioDto
                            {
                                ltg_estatus = reader.GetInt32(0),
                                ltg_description = reader.IsDBNull(1) ? null : reader.GetString(1),
                            });
                        }

                  
                        await reader.NextResultAsync();
                        while (await reader.ReadAsync())
                        {
                            datos.Salas.Add(new Salao
                            {
                                IdSala = reader.IsDBNull(1) ? null : reader.GetInt32(0),
                                Nombre = reader.IsDBNull(1) ? null : reader.GetString(1),
                                IdTribunal = reader.IsDBNull(1) ? null : reader.GetInt32(2),
                            });
                        }
                    }
                }
            }

            return Ok(datos);
        }


        [HttpGet("SeguimientoPaginado")]
        public async Task<IActionResult> ObtenerLitigiosDetalladosPaginado(
    int page = 1,
    int pageSize = 10,
    string? search = null,
    string? estatus = null 
)
        {
            var result = new LitigiosDigitadoresPaginadoResponse();

            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
            using var command = new SqlCommand("sp_ObtenerLitigiosDetalladosSeguimiento", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Page", page);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(search) ? DBNull.Value : search);
            command.Parameters.AddWithValue("@EstatusFiltro", string.IsNullOrWhiteSpace(estatus) ? DBNull.Value : estatus); // 👈 Esto faltaba

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                result.Total = reader["Total"] != DBNull.Value ? Convert.ToInt32(reader["Total"]) : 0;

            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var litigio = new LitigioDetallado
                    {
                        id_Ltg = Convert.ToInt32(reader["id_Ltg"]),
                        ltg_acto = reader["ltg_acto"]?.ToString(),
                        ltg_Fecha_Acto = reader["ltg_Fecha_Acto"] as DateTime?,
                        ltg_Cedula_Demandante = reader["ltg_Cedula_Demandante"]?.ToString(),
                        ltg_Nombre_Demandante = reader["ltg_Nombre_Demandante"]?.ToString(),
                        TipoDemanda_Nombre = reader["TipoDemanda_Nombre"]?.ToString(),
                        Estatus_Descripcion = reader["Estatus_Descripcion"]?.ToString(),
                        ltg_Fecha_Audiencia = reader["ltg_Fecha_Audiencia"] as DateTime?
                    };

                    result.Litigios.Add(litigio);
                }
            }

            return Ok(result);
        }




        //Api para paginar los litigios de 10 en 10 en la bandeja de gestion de litigios
        [HttpGet("litigios-paginados")]
        public async Task<IActionResult> ObtenerLitigiosPaginados(
        string tipo = "asignado",
        int page = 1,
        int pageSize = 10,
         string? search = null)
        {
            var result = new LitigiosResponse();

            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
            using var command = new SqlCommand("sp_ObtenerLitigiosDetalladosSupervisorPaginado", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Page", page);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Tipo", tipo);
            command.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? DBNull.Value : search);


            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            // Primer resultado: totales
            if (await reader.ReadAsync())
            {
                result.TotalAsignados = reader.GetInt32(reader.GetOrdinal("TotalAsignados"));
                result.TotalSinAsignar = reader.GetInt32(reader.GetOrdinal("TotalSinAsignar"));
            }

            // Segundo resultado: solo la lista correspondiente al tipo
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var litigio = LeerLitigioDesdeReader(reader);

                    if (tipo == "asignado")
                        result.Asignados.Add(litigio);
                    else
                        result.SinAsignar.Add(litigio);
                }
            }

            return Ok(result);
        }


        //Api para paginar los litigios de 10 en 10 en la bandeja de los abogados 
        [HttpGet("Litigio_AsignacionesPaginado")]
        public async Task<IActionResult> ObtenerLitigiosAsignadosPaginado(int idUsuario, int page = 1, int pageSize = 10, string? search = null)
        {
            var result = new LitigiosAsignadosPaginadoResponse();

            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
            using var command = new SqlCommand("sp_ObtenerLitigiosAsignadosPaginado", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@idUsuario", idUsuario);
            command.Parameters.AddWithValue("@Page", page);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? DBNull.Value : search);


            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                result.Total = reader.GetInt32(reader.GetOrdinal("Total"));

            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    result.Litigios.Add(new LitigiosAsignadosAbogados
                    {
                        id_Ltg = reader.GetInt32(reader.GetOrdinal("id_Ltg")),
                        ltg_acto = reader["ltg_acto"]?.ToString(),
                        ltg_Fecha_Acto = reader["ltg_Fecha_Acto"] as DateTime?,
                        ltg_Cedula_Demandante = reader["ltg_Cedula_Demandante"]?.ToString(),
                        ltg_Nombre_Demandante = reader["ltg_Nombre_Demandante"]?.ToString(),
                        Nombre_Tipo_Demanda = reader["nombre_Tipo_Demanda"]?.ToString(),
                        ltg_Fecha_Audiencia = reader["ltg_Fecha_Audiencia"] as DateTime?,
                        ltg_description = reader["ltg_description"]?.ToString()
                    });
                }
            }

            return Ok(result);
        }

        public class LitigiosAsignadosPaginadoResponse
        {
            public int Total { get; set; }
            public List<LitigiosAsignadosAbogados> Litigios { get; set; } = new();
        }


        /// <summary>
        /// Nos permite traer los litigos de 10 en 10 para no sobre cargar la consulta
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        [HttpGet("Litigio_detalladoDigitadorPaginado")]
        public async Task<IActionResult> ObtenerLitigiosDigitadoresPaginado(int page = 1, int pageSize = 10, string? search = null)
        {
            var result = new LitigiosDigitadoresPaginadoResponse();

            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
            using var command = new SqlCommand("ObtenerLitigiosDigitadoresPaginado", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Page", page);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? DBNull.Value : search);


            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            // Leer total
            if (await reader.ReadAsync())
            {
                result.Total = reader["Total"] != DBNull.Value ? Convert.ToInt32(reader["Total"]) : 0;
            }

            // Leer resultados de litigios
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var litigio = new LitigioDetallado
                    {
                        id_Ltg = reader["id_Ltg"] != DBNull.Value ? Convert.ToInt32(reader["id_Ltg"]) : 0,
                        ltg_acto = reader["ltg_acto"]?.ToString(),
                        ltg_Fecha_Acto = reader["ltg_Fecha_Acto"] as DateTime?,
                        ltg_Cedula_Demandante = reader["ltg_Cedula_Demandante"]?.ToString(),
                        ltg_Nombre_Demandante = reader["ltg_Nombre_Demandante"]?.ToString(),
                        TipoDemanda_Nombre = reader["TipoDemanda_Nombre"]?.ToString(),
                        ltg_Fecha_Audiencia = reader["ltg_Fecha_Audiencia"] as DateTime?,
                        Estatus_Descripcion = reader["Estatus_Descripcion"]?.ToString()
                    };

                    result.Litigios.Add(litigio);
                }
            }

            return Ok(result);
        }


        public class LitigiosDigitadoresPaginadoResponse
        {
            public int Total { get; set; }
            public List<LitigioDetallado> Litigios { get; set; } = new();
        }


        private LitigioDetalladoS LeerLitigioDesdeReader(SqlDataReader reader)
        {
            return new LitigioDetalladoS
            {
                id_Ltg = reader.GetInt32(reader.GetOrdinal("id_Ltg")),
                ltg_acto = reader["ltg_acto"]?.ToString(),
                ltg_Fecha_Acto = reader["ltg_Fecha_Acto"] as DateTime?,
                ltg_Cedula_Demandante = reader["ltg_Cedula_Demandante"]?.ToString(),
                ltg_Nombre_Demandante = reader["ltg_Nombre_Demandante"]?.ToString(),
                TipoDemanda_Nombre = reader["TipoDemanda_Nombre"]?.ToString(),
                Estatus_Descripcion = reader["Estatus_Descripcion"]?.ToString(),
                ltg_Fecha_Audiencia = reader["ltg_Fecha_Audiencia"] as DateTime?,
                Asignado = Convert.ToBoolean(reader["Asignado"])
            };
        }



        
       

        /// <summary>
        /// Trae la informacion del litigio seleccionado para llenar 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("detallados/{id}")]
        public async Task<ActionResult<LitigioDetallado>> ObtenerLitigioPorId(int id)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));

            var parametros = new DynamicParameters();
            parametros.Add("@IdLitigio", id);

            var litigio = await connection.QueryFirstOrDefaultAsync<LitigioDetallado>(
                "sp_ObtenerLitigioPorId",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            if (litigio == null)
            {
                return NotFound();
            }

            return Ok(litigio);
        }

        /// <summary>
        /// Nos permite buscar los litigios por el numero de acto o la cedula 
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        [HttpGet("Buscar")]
        public async Task<IActionResult> BuscarPorCedulaOActo(string valor)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));

            var parametrosBusqueda = new DynamicParameters();
            parametrosBusqueda.Add("@valor", valor.Trim());

            var litigioBase = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_BuscarLitigioPorCedulaOActo",
                parametrosBusqueda,
                commandType: CommandType.StoredProcedure
            );

            if (litigioBase == null)
                return NotFound("No se encontró litigio con ese valor.");

            int id_Ltg = litigioBase.id_Ltg;

            var parametrosDetalle = new DynamicParameters();
            parametrosDetalle.Add("@IdLitigio", id_Ltg);

            var litigioCompleto = await connection.QueryFirstOrDefaultAsync<LitigioDetallado>(
                "sp_ObtenerLitigioPorId",
                parametrosDetalle,
                commandType: CommandType.StoredProcedure
            );

            if (litigioCompleto == null)
                return NotFound("No se encontró información detallada para el litigio.");

            return Ok(litigioCompleto);
        }



        /// <summary>
        /// Nos trae la informacion guardada en la tabla historico_litigio para cargar la linea de tiempo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("historial/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHistorialLitigio(int id)
            {
                var result = new List<LineaTiempoItemDto>();

                var connectionString = _configuration.GetConnectionString("Sistema_Legal");
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT h.Tipo_cambio, h.Valor_anterior, h.Valor_nuevo, h.Fecha_cambio, u.nombreUsuario AS UsuarioNombre
                    FROM Historico_Litigio h
                    LEFT JOIN Usuarios u ON h.Usuario_id = u.idUsuario
                    WHERE h.Id_litigio = @Id
                    ORDER BY h.Fecha_cambio ASC", connection);
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var tipo = reader["Tipo_cambio"].ToString();
                    var anterior = reader["Valor_anterior"]?.ToString();
                    var nuevo = reader["Valor_nuevo"]?.ToString();
                    var fecha = ((DateTime)reader["Fecha_cambio"]).ToString("o"); // "o" = ISO 8601
                    var usuarioNombre = reader["UsuarioNombre"]?.ToString();



                result.Add(new LineaTiempoItemDto
                    {
                        Status = tipo,
                        Date = fecha,
                        Content = anterior != null ? $"-{anterior}-{nuevo}-" : nuevo,
                        Icon = IconoPorTipo(tipo),
                        Color = ColorPorTipo(tipo),
                        Usuario = usuarioNombre
                });
                }

                return Ok(result);
            }

            private string IconoPorTipo(string tipo)
            {
                return tipo switch


              
                {
                    
                    "Creación del litigio" => "pi pi-building",
                    "Nueva Audiencia" => "pi pi-calendar-plus",
                    "Actualización de Audiencia" => "pi pi-calendar",

                    "Nueva Evidencia" => "pi pi-file-pdf",
                    "Cambio de tribunal" => "pi pi-building",
                    "Cambio de estatus" => "pi pi-info-circle",
                    "Cambio de sentencia" => "pi pi-check",
                    "Inicio del litigio" => "pi pi-flag",
                    "Cambio de representante" => "pi pi-user-edit",

                    _ => "pi-exclamation-triangle"
                };
            }

            private string ColorPorTipo(string tipo)
            {
                return tipo switch
                {
                    "Nueva Audiencia" => "#75F07B",
                    "Actualización de Audiencia" => "#75F0B2",
                    "Nueva Evidencia" => "#E02500",
                    "Cambio de tribunal" => "##C5F0DE",
                    "Cambio de estatus" => "#FFC107",
                    "Cambio de sentencia" => "#4CAF50",
                    "Inicio del litigio" => "#9C27B0",
                    "Cambio de representante" => "#CABBE1",
                    _ => "#00254F"
                };
            }
        }




    }





