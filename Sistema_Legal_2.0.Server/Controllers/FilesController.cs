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



        [HttpPost("SubirEvidencia")]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueCountLimit = int.MaxValue)]
        public async Task<IActionResult> SubirEvidencia([FromForm] EvidenciasUploadModel evidencia)
        {
            if (evidencia.Archivo == null || evidencia.Archivo.Length == 0)
                return BadRequest("No se recibió ningún archivo.");

            string nombreCarpetaLitigio = "";

            // Obtener el nombre de la carpeta (ltg_acto) desde la tabla Litigios
            using (SqlConnection connection = new SqlConnection(_cadenaSQL))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand(@"
            SELECT L.ltg_acto 
            FROM Ruta_archivos R
            INNER JOIN Litigios L ON R.id_Ltg = L.id_Ltg
            WHERE R.id_Ltg = @idLtg", connection))
                {
                    cmd.Parameters.AddWithValue("@idLtg", evidencia.id_Ltg);

                    var result = await cmd.ExecuteScalarAsync();
                    if (result == null)
                        return NotFound("No se encontró el litigio asociado.");

                    nombreCarpetaLitigio = result.ToString();
                }
            }

            string rutaBase = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";
            string rutaLitigio = Path.Combine(rutaBase, nombreCarpetaLitigio);
            string rutaEvidencia = Path.Combine(rutaLitigio, "Evidencias");

            if (!Directory.Exists(rutaEvidencia))
                Directory.CreateDirectory(rutaEvidencia);

            string nombreArchivo = Path.GetFileName(evidencia.Archivo.FileName);
            string rutaCompleta = Path.Combine(rutaEvidencia, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await evidencia.Archivo.CopyToAsync(stream);
            }

            string rutaRelativa = Path.Combine(nombreCarpetaLitigio, "Evidencias", nombreArchivo);

            using (SqlConnection connection = new SqlConnection(_cadenaSQL))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_SubirEvidencia", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_litigio", evidencia.id_Ltg);
                    cmd.Parameters.AddWithValue("@ruta_archivo", rutaRelativa);
                    cmd.Parameters.AddWithValue("@nombre_archivo", nombreArchivo);
                    cmd.Parameters.AddWithValue("@id_usuario", evidencia.id_usuario);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return Ok(new { mensaje = "Evidencia subida correctamente.", rutaRelativa });
        }

      




        [HttpGet("rutas/{id}")]
        public async Task<IActionResult> ObtenerRutasPorLitigio(int id)
        {
            var parametros = new DynamicParameters();
            parametros.Add("@IdLitigio", id);

            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
            var rutas = await connection.QueryAsync<Ruta_archivos>(
                "sp_ObtenerRutasPorLitigio",
                parametros,
                commandType: CommandType.StoredProcedure
            ); 

            return Ok(rutas);
        }








        [HttpGet("rutaspor/{id}")]
        public async Task<IActionResult> GetRutaPorId(int id)
        {
            var ruta = await _db_silegContext.Ruta_archivos
                                     .Where(r => r.id_Ruta == id)
                                     .Select(r => r.Ruta)
                                     .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(ruta))
            {
                return NotFound("Ruta no encontrada para el ID proporcionado.");
            }

            // Construimos la ruta completa del archivo
            var basePath = _configuration.GetValue<string>("FilesServerPath");
            var rutaArchivoCompleta = Path.Combine(basePath, ruta); // ahora ruta es sólo el nombre de carpeta/archivo

            if (!System.IO.File.Exists(rutaArchivoCompleta))
            {
                return NotFound("El archivo no existe en el servidor.");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(rutaArchivoCompleta, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var contentType = GetContentType(rutaArchivoCompleta);  // Aquí se usa el método GetContentType
            var nombreArchivo = Path.GetFileName(rutaArchivoCompleta);

            return File(memory, contentType, nombreArchivo);
        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream",
            };
        }










        [HttpGet("comentarios-evidencias/{idLitigio}")]
        public async Task<IActionResult> ObtenerComentariosEvidenciasPorLitigio(int idLitigio)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
            var parametros = new DynamicParameters();
            parametros.Add("@IdLitigio", idLitigio);

            var resultado = await connection.QueryAsync<dynamic>(
                "sp_OComentariosEvidenciasID",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return Ok(resultado);
        }










        [HttpPost("subir-evidencia-comentario")]
        public async Task<IActionResult> SubirEvidenciaYComentario([FromForm] EvidenciaComentarioDTO modelo)
        {
            var archivo = modelo.Archivo;
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Archivo no proporcionado.");

            var basePath = _configuration.GetValue<string>("FilesServerPath");
            var nombreOriginal = Path.GetFileNameWithoutExtension(archivo.FileName);
            var extension = Path.GetExtension(archivo.FileName);
            var fechaActual = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var nombreCarpeta = $"{nombreOriginal}_{fechaActual}";
            var carpetaDestino = Path.Combine(basePath, nombreCarpeta);

            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            var rutaArchivo = Path.Combine(carpetaDestino, archivo.FileName);

            using (var stream = new FileStream(rutaArchivo, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
            var parametros = new DynamicParameters();
            parametros.Add("@IdUsuario", modelo.IdUsuario);
            parametros.Add("@IdLitigio", modelo.IdLitigio);
            parametros.Add("@TextoComentario", modelo.Comentario);
            parametros.Add("@NombreArchivo", archivo.FileName);
            parametros.Add("@RutaArchivo", Path.Combine(nombreCarpeta, archivo.FileName));
            parametros.Add("@ComentarioId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parametros.Add("@EvidenciaId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("sp_InsertarComentarioYArchivo", parametros, commandType: CommandType.StoredProcedure);

            return Ok(new
            {
                mensaje = "Evidencia y comentario guardados correctamente.",
                comentarioId = parametros.Get<int>("@ComentarioId"),
                evidenciaId = parametros.Get<int>("@EvidenciaId")
            });
        }




    }

}



