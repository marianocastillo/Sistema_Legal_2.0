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


        //[HttpPost]
        //[Route("Subir")]
        //[DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueCountLimit = int.MaxValue)]
        //public IActionResult Subir([FromForm] Ruta_archivos request)
        //{
        //    string rutaDocumento = Path.Combine(_filesServerPath, request.Archivo.FileName);

        //    try
        //    {
        //        using (FileStream newfile = System.IO.File.Create(rutaDocumento))
        //        {
        //            request.Archivo.CopyTo(newfile);
        //            newfile.Flush();
        //        }
        //        using (var conexion = new SqlConnection(_cadenaSQL))
        //        {
        //            conexion.Open();
        //            var cmd = new SqlCommand("sp_guardar_documento", conexion);
        //            cmd.Parameters.AddWithValue("nombre", request.Nombre);
        //            cmd.Parameters.AddWithValue("ruta", rutaDocumento);

        //            //cmd.Parameters.AddWithValue("", request.id_usuario);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.ExecuteNonQuery();
        //        }

        //        return StatusCode(StatusCodes.Status200OK, new { mensaje = "documento guardado" });
        //    }
        //    catch (Exception error)
        //    {
        //        return StatusCode(StatusCodes.Status200OK, new { mensaje = error.Message });
        //    }
        //}


        //[HttpPost("Subir_Litigio")]
        //public async Task<IActionResult> CrearLitigio([FromBody] Litigio litigio)
        //{
        //    using (SqlConnection connection = new SqlConnection(_cadenaSQL))
        //    {
        //        using (SqlCommand command = new SqlCommand("sp_CrearLitigio", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@ltg_acto", litigio.ltg_acto);
        //            command.Parameters.AddWithValue("@ltg_Fecha_Acto", litigio.ltg_Fecha_Acto);
        //            command.Parameters.AddWithValue("@id_Tipo_Demanda", litigio.id_Tipo_Demanda);
        //            command.Parameters.AddWithValue("@ltg_Cedula_Demandante", litigio.ltg_Cedula_Demandante);
        //            command.Parameters.AddWithValue("@ltg_Nacionalidad", litigio.ltg_Nacionalidad);
        //            command.Parameters.AddWithValue("@ltg_Demandante", litigio.ltg_Demandante);
        //            command.Parameters.AddWithValue("@ltg_Tipo_Demandante", litigio.ltg_Tipo_Demandante);
        //            command.Parameters.AddWithValue("@ltg_Cedula_Representante", litigio.ltg_Cedula_Representante);
        //            command.Parameters.AddWithValue("@ltg_Nombre_Representante", litigio.ltg_Nombre_Representante);
        //            command.Parameters.AddWithValue("@ltg_Fecha_Audiencia", litigio.ltg_Fecha_Audiencia);
        //            command.Parameters.AddWithValue("@ltg_Doc_Demandante", litigio.ltg_Doc_Demandante);
        //            command.Parameters.AddWithValue("@ltg_Fecha_Actualizacion", litigio.ltg_Fecha_Actualizacion);
        //            command.Parameters.AddWithValue("@id_Tribunal", litigio.id_Tribunal);
        //            command.Parameters.AddWithValue("@id_Sentencia", litigio.id_Sentencia);
        //            command.Parameters.AddWithValue("@id_usuario", litigio.id_usuario);
        //            command.Parameters.AddWithValue("@id_ruta", litigio.id_ruta);
        //            command.Parameters.AddWithValue("@id_Estatus", litigio.id_Estatus);

        //            await connection.OpenAsync();
        //            await command.ExecuteNonQueryAsync();
        //        }
        //    }
        //    return CreatedAtAction(nameof(CrearLitigio), litigio);
        //}



        //[HttpPost("SubirArchivo")]
        //[DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueCountLimit = int.MaxValue)]
        //public async Task<IActionResult> SubirArchivo(IFormFile archivo, [FromForm] string nombreCarpeta)
        //{
        //    if (archivo == null || archivo.Length == 0)
        //        return BadRequest("No se recibió ningún archivo.");

        //    // Ruta absoluta personalizada donde guardarás los archivos
        //    string rutaBase = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";

        //    // Crear carpeta dentro de esa ruta
        //    string rutaCarpeta = Path.Combine(rutaBase, nombreCarpeta);
        //    if (!Directory.Exists(rutaCarpeta))
        //        Directory.CreateDirectory(rutaCarpeta);

        //    // Guardar el archivo en la carpeta creada
        //    string nombreArchivo = Path.GetFileName(archivo.FileName);
        //    string rutaArchivoCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

        //    using (var stream = new FileStream(rutaArchivoCompleta, FileMode.Create))
        //    {
        //        await archivo.CopyToAsync(stream);
        //    }

        //    // Ruta relativa que guardarás en la base de datos
        //    string rutaRelativa = Path.Combine(nombreCarpeta, nombreArchivo);

        //    // Guardar en la base de datos
        //    using (SqlConnection connection = new SqlConnection(_cadenaSQL))
        //    {
        //        string query = "INSERT INTO Ruta_archivos (Ruta, Nombre, id_usuario) VALUES (@Ruta, @Nombre, @id_usuario)";
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Ruta", rutaRelativa);
        //            command.Parameters.AddWithValue("@Nombre", nombreArchivo);
        //            command.Parameters.AddWithValue("@id_usuario", 1); // o el usuario autenticado
        //            await connection.OpenAsync();
        //            await command.ExecuteNonQueryAsync();
        //        }
        //    }

        //    return Ok(new { rutaRelativa, nombreArchivo });
        //}





        //[HttpGet("ObtenerPorNombre")]
        //public IActionResult ObtenerDocumentosPorDescripcion(string nombre)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(_cadenaSQL))
        //        {
        //            connection.Open();
        //            using (SqlCommand cmd = new SqlCommand("sp_obtener_documentos_por_nombre", connection))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@nombre", nombre);


        //                SqlDataReader reader = cmd.ExecuteReader();
        //                List<object> documentos = new List<object>();

        //                while (reader.Read())
        //                {
        //                    documentos.Add(new
        //                    {
        //                        Nombre = reader["nombre"].ToString(),
        //                        Ruta = reader["Ruta"].ToString()
        //                    });
        //                }

        //                return Ok(documentos);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { mensaje = ex.Message });
        //    }
        //}




        //[HttpGet("archivos/{nombreArchivo}")]
        //public IActionResult ObtenerArchivo(string nombreArchivo)
        //{
        //    try
        //    {
        //        string rutaCarpeta = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";  // Ruta donde guardas los archivos
        //        string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

        //        if (!System.IO.File.Exists(rutaCompleta))
        //        {
        //            return NotFound(new { mensaje = "Archivo no encontrado" });
        //        }

        //        string tipoMime = "application/octet-stream"; // Tipo MIME por defecto
        //        string extension = Path.GetExtension(rutaCompleta).ToLower();

        //        // Asignar tipos MIME comunes
        //        var tiposMime = new Dictionary<string, string>
        //{
        //    { ".pdf", "application/pdf" },
        //    { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        //    { ".jpg", "image/jpeg" },
        //    { ".png", "image/png" }
        //};

        //        if (tiposMime.ContainsKey(extension))
        //        {
        //            tipoMime = tiposMime[extension];
        //        }

        //        var archivoBytes = System.IO.File.ReadAllBytes(rutaCompleta);
        //        return File(archivoBytes, tipoMime);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { mensaje = ex.Message });
        //    }
        //}

        [HttpPost("Subir_Litigio_Con_Archivo")]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueCountLimit = int.MaxValue)]
        public async Task<IActionResult> SubirLitigioConArchivo([FromForm] LitigioConArchivo datos)
        {
            if (datos.Archivo == null || datos.Archivo.Length == 0)
                return BadRequest("No se recibió ningún archivo.");

            string rutaBase = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";
            string rutaActo = Path.Combine(rutaBase, datos.ltg_acto);
            string rutaFinal = Path.Combine(rutaActo, datos.NombreCarpeta);

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

                    var result = await command.ExecuteScalarAsync();
                    return Ok(new { mensaje = "Litigio y archivo subidos correctamente.", id_litigio = result, rutaRelativa });
                }
            }
        }


        [HttpPost("SubirEvidencia")]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueCountLimit = int.MaxValue)]
        public async Task<IActionResult> SubirEvidencia([FromForm] EvidenciasUploadModel evidencia)
        {
            if (evidencia.Archivo == null || evidencia.Archivo.Length == 0)
                return BadRequest("No se recibió ningún archivo.");

            string rutaBase = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";

            // Obtener la ruta base del litigio (simulamos acto/carpeta por ahora)
            string rutaLitigio = Path.Combine(rutaBase,  evidencia.NombreCarpeta);

            // Ruta final: ...\Evidencias
            string rutaEvidencia = Path.Combine(rutaLitigio, "Evidencias");

            if (!Directory.Exists(rutaEvidencia))
                Directory.CreateDirectory(rutaEvidencia);

            // Guardar el archivo
            string nombreArchivo = Path.GetFileName(evidencia.Archivo.FileName);
            string rutaCompleta = Path.Combine(rutaEvidencia, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await evidencia.Archivo.CopyToAsync(stream);
            }

            string rutaRelativa = Path.Combine( evidencia.NombreCarpeta, "Evidencias", nombreArchivo);

            using (SqlConnection connection = new SqlConnection(_cadenaSQL))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_SubirEvidencia", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_litigio", evidencia.id_litigio);
                    cmd.Parameters.AddWithValue("@ruta_archivo", rutaRelativa);
                    cmd.Parameters.AddWithValue("@nombre_archivo", nombreArchivo);
                    cmd.Parameters.AddWithValue("@id_usuario", evidencia.id_usuario);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return Ok(new { mensaje = "Evidencia subida correctamente.", rutaRelativa });
        }



        //[HttpGet("ObtenerArchivosPorActo")]
        //public IActionResult ObtenerArchivosPorActo([FromQuery] string nombreActo)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(nombreActo))
        //            return BadRequest("Debe proporcionar el nombre del acto.");

        //        string rutaBase = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";
        //        string rutaActo = Path.Combine(rutaBase, nombreActo);

        //        if (!Directory.Exists(rutaActo))
        //            return NotFound("No existe ninguna carpeta para ese acto.");

        //        // Obtener todos los archivos incluyendo los de las subcarpetas
        //        var archivos = Directory.GetFiles(rutaActo, "*.*", SearchOption.AllDirectories)
        //                                .Select(path => new
        //                                {
        //                                    NombreArchivo = Path.GetFileName(path),
        //                                    RutaRelativa = Path.GetRelativePath(rutaBase, path),
        //                                    RutaCompleta = path
        //                                })
        //                                .ToList();

        //        return Ok(archivos);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error al obtener: {{ex.Message}}");
        //            }

        //}
        /// <summary>
        /// Obtiene los litigios con información detallada desde el procedimiento almacenado.
        /// </summary>
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

        //[HttpGet("documento/{id}")]
        //public async Task<IActionResult> AbrirDocumento(int id)
        //{
        //    try
        //    {
        //        // 1. Obtener datos desde BD
        //        var parametros = new DynamicParameters();
        //        parametros.Add("@IdRuta", id);

        //        using var connection = new SqlConnection(_configuration.GetConnectionString("Sistema_Legal"));
        //        var resultado = await connection.QueryFirstOrDefaultAsync<(string Ruta, string Nombre)>(
        //            "sp_ObtenerRutaPorId",
        //            parametros,
        //            commandType: CommandType.StoredProcedure
        //        );

        //        if (resultado == default)
        //            return NotFound("Registro no encontrado en BD");

        //        // 2. Construir ruta válida
        //        string rutaBase = _configuration["RutaBaseDocumentos"] ?? @"C:\Users\mariancastillo\Desktop\SistemaLitigio";
        //        string rutaRelativa = resultado.Ruta.Trim().Replace('/', '\\'); // Normalizar separadores

        //        // Eliminar caracteres inválidos
        //        foreach (char c in Path.GetInvalidPathChars())
        //            rutaRelativa = rutaRelativa.Replace(c.ToString(), "");

        //        string rutaCompleta = Path.Combine(rutaBase, rutaRelativa);
        //        rutaCompleta = Path.GetFullPath(rutaCompleta); // Resuelve .\ o ..\

        //        // 3. Verificaciones de seguridad
        //        if (!rutaCompleta.StartsWith(rutaBase))
        //            return BadRequest("Ruta no permitida");

        //        // 4. Verificar existencia
        //        if (!File.Exists(rutaCompleta))
        //        {
        //            // Log detallado para diagnóstico
        //            var debugInfo = new
        //            {
        //                RutaBase = rutaBase,
        //                RutaRelativa = resultado.Ruta,
        //                RutaCompletaGenerada = rutaCompleta,
        //                RutaRealExistente = Directory.GetFiles(Path.GetDirectoryName(rutaCompleta))
        //            };

        //            return NotFound(new
        //            {
        //                Message = "Archivo no encontrado en ruta generada",
        //                Debug = debugInfo
        //            });
        //        }

        //        // 5. Devolver datos del archivo
        //        var fileInfo = new FileInfo(rutaCompleta);
        //        return Ok(new
        //        {
        //            RutaFisica = rutaCompleta,
        //            NombreArchivo = resultado.Nombre,
        //            TamañoBytes = fileInfo.Length,
        //            FechaModificacion = fileInfo.LastWriteTime,
        //            TipoMIME = MimeMapping.GetMimeMapping(rutaCompleta)
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Error = ex.Message,
        //            Detalle = ex.InnerException?.Message,
        //            StackTrace = ex.StackTrace
        //        });
        //    }
        //}


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











    }

}



