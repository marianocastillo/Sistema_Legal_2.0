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


        [HttpPut("EditarLitigio")]
        public async Task<IActionResult> EditarLitigio([FromBody] LitigioEditarDto litigio)
        {
            using (SqlConnection connection = new SqlConnection(_cadenaSQL))
            {
                await connection.OpenAsync();

                litigio.ltg_acto = string.IsNullOrWhiteSpace(litigio.ltg_acto) ? null : litigio.ltg_acto;
                litigio.ltg_Cedula_Demandante = string.IsNullOrWhiteSpace(litigio.ltg_Cedula_Demandante) ? null : litigio.ltg_Cedula_Demandante;
                litigio.ltg_Nacionalidad = string.IsNullOrWhiteSpace(litigio.ltg_Nacionalidad) ? null : litigio.ltg_Nacionalidad;
                litigio.ltg_Demandante = string.IsNullOrWhiteSpace(litigio.ltg_Demandante) ? null : litigio.ltg_Demandante;
                litigio.ltg_Tipo_Demandante = string.IsNullOrWhiteSpace(litigio.ltg_Tipo_Demandante) ? null : litigio.ltg_Tipo_Demandante;
                litigio.ltg_Cedula_Representante = string.IsNullOrWhiteSpace(litigio.ltg_Cedula_Representante) ? null : litigio.ltg_Cedula_Representante;
                litigio.ltg_Nombre_Representante = string.IsNullOrWhiteSpace(litigio.ltg_Nombre_Representante) ? null : litigio.ltg_Nombre_Representante;


                using (SqlCommand cmd = new SqlCommand("sp_EditarLitigio", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_Ltg", litigio.id_Ltg);
                    cmd.Parameters.AddWithValue("@ltg_acto", (object?)litigio.ltg_acto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Fecha_Acto", litigio.ltg_Fecha_Acto);
                    cmd.Parameters.AddWithValue("@id_Tipo_Demanda", litigio.id_Tipo_Demanda);
                    cmd.Parameters.AddWithValue("@ltg_Cedula_Demandante", (object?)litigio.ltg_Cedula_Demandante?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Nacionalidad", (object?)litigio.ltg_Nacionalidad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Demandante", (object?)litigio.ltg_Demandante ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Tipo_Demandante", litigio.ltg_Tipo_Demandante);
                    cmd.Parameters.AddWithValue("@ltg_Cedula_Representante", (object?)litigio.ltg_Cedula_Representante ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Nombre_Representante", (object?)litigio.ltg_Nombre_Representante ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Fecha_Audiencia", (object?)litigio.ltg_Fecha_Audiencia ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ltg_Fecha_Actualizacion", litigio.ltg_Fecha_Actualizacion);
                    cmd.Parameters.AddWithValue("@id_Tribunal", litigio.id_Tribunal);
                    cmd.Parameters.AddWithValue("@id_Sentencia", litigio.id_Sentencia);
                    cmd.Parameters.AddWithValue("@id_usuario", litigio.id_usuario);
                    cmd.Parameters.AddWithValue("@id_Estatus", litigio.id_Estatus);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                        return NotFound("Litigio no encontrado o no modificado.");

                    return Ok(new { mensaje = "Litigio actualizado correctamente." });
                }
            }
        }


        [HttpPost("Subir_Litigio_Con_Archivo")]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueCountLimit = int.MaxValue)]
        public async Task<IActionResult> SubirLitigioConArchivo([FromForm] LitigioConArchivo datos)
        {
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(datos));

            if (datos.Archivo == null || datos.Archivo.Length == 0)
                return BadRequest("No se recibió ningún archivo.");

            string rutaBase = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";
            string rutaActo = Path.Combine(rutaBase, datos.ltg_acto);
            string rutaFinal = Path.Combine(rutaActo, datos.ltg_acto);

            if (!Directory.Exists(rutaFinal))
                Directory.CreateDirectory(rutaFinal);

            string nombreArchivo = Path.GetFileName(datos.Archivo.FileName);
            string rutaArchivoCompleta = Path.Combine(rutaFinal, nombreArchivo);

            using (var stream = new FileStream(rutaArchivoCompleta, FileMode.Create))
            {
                await datos.Archivo.CopyToAsync(stream);
            }

            // Ruta relativa para guardar en BD
            string rutaRelativa = Path.Combine(datos.ltg_acto, datos.NombreCarpeta, nombreArchivo);

            using (SqlConnection connection = new SqlConnection(_cadenaSQL))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_CrearLitigio", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ltg_acto", datos.ltg_acto);
                    command.Parameters.AddWithValue("@ltg_Fecha_Acto", datos.ltg_Fecha_Acto);
                    command.Parameters.AddWithValue("@id_Tipo_Demanda", datos.id_Tipo_Demanda);
                    command.Parameters.AddWithValue("@ltg_Cedula_Demandante", datos.ltg_Cedula_Demandante);
                    command.Parameters.AddWithValue("@ltg_Nacionalidad", datos.ltg_Nacionalidad);
                    command.Parameters.AddWithValue("@ltg_Demandante", datos.ltg_Demandante);
                    command.Parameters.AddWithValue("@ltg_Tipo_Demandante", datos.ltg_Tipo_Demandante);
                    command.Parameters.AddWithValue("@ltg_Cedula_Representante", datos.ltg_Cedula_Representante);
                    command.Parameters.AddWithValue("@ltg_Nombre_Representante", datos.ltg_Nombre_Representante);
                    command.Parameters.AddWithValue("@ltg_Fecha_Audiencia", (object?)datos.ltg_Fecha_Audiencia ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ltg_Fecha_Actualizacion", (object?)datos.ltg_Fecha_Actualizacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_Tribunal", (object?)datos.id_Tribunal ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_Sentencia", (object?)datos.id_Sentencia ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_usuario", datos.id_usuario);
                    command.Parameters.AddWithValue("@id_Estatus", datos.id_Estatus);
                    command.Parameters.AddWithValue("@ruta_archivo", rutaRelativa);
                    command.Parameters.AddWithValue("@nombre_archivo", nombreArchivo);
                    command.Parameters.AddWithValue("@comentario", datos.comentario);
                    var result = await command.ExecuteScalarAsync();
                    return Ok(new { mensaje = "Litigio y archivo subidos correctamente.", id_litigio = result, rutaRelativa });
                }
            }
        }

      
        [HttpGet("Litigio_detallado")]
        public async Task<ActionResult<IEnumerable<LitigioDetallado>>> ObtenerLitigiosDetallados()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));

            var resultado = await connection.QueryAsync<LitigioDetallado>(
                "sp_ObtenerLitigiosDetallados",
                commandType: CommandType.StoredProcedure
            );
            
            return Ok(resultado);
        }



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



        [HttpGet("comentarios/{id}")]
        public async Task<IActionResult> ObtenerComentariosPorLitigio(int id)
        {
            var parametros = new DynamicParameters();
            parametros.Add("@IdLitigio", id);

            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
            var comentarios = await connection.QueryAsync<ComentariosLitigio>(
                "sp_ObtenerComentariosPorID",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return Ok(comentarios);
        }



        [HttpGet("datos-litigio")]
        public async Task<IActionResult> ObtenerDatosLitigio()
        {
            var datos = new DatosLitigioDto
            {
                Tribunales = new List<TribunalDto>(),
                TiposDemanda = new List<TipoDemandaDto>(),
                EstatusLitigios = new List<EstatusLitigioDto>()
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
                        // Leer tribunales
                        while (await reader.ReadAsync())
                        {
                            datos.Tribunales.Add(new TribunalDto
                            {
                                Id_Tribunal = reader.GetInt32(0),
                                Nombre_Tribunal = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                Telefono = reader.GetString(3),
                                Latitud = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                                Longitud = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                                IdEstatus = reader.GetInt32(6),
                                Direccion = reader.GetString(7)
                            });
                        }

                        // Siguiente resultset
                        await reader.NextResultAsync();

                        // Leer tipos de demanda
                        while (await reader.ReadAsync())
                        {
                            datos.TiposDemanda.Add(new TipoDemandaDto
                            {
                                id_demanda = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                id_Estatus = reader.GetInt32(2)
                            });
                        }

                        // Siguiente resultset
                        await reader.NextResultAsync();

                        // Leer estatus de litigios
                        while (await reader.ReadAsync())
                        {
                            datos.EstatusLitigios.Add(new EstatusLitigioDto
                            {
                                ltg_estatus = reader.GetInt32(0),
                                ltg_description = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return Ok(datos);
        }
        [HttpGet("Buscar")]
        public async Task<IActionResult> BuscarPorCedulaOActo(string valor)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));

            // 1. Buscar ID del litigio usando el valor (cédula o número de acto)
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

            // 2. Obtener los datos completos usando el nuevo SP
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







    }

}



