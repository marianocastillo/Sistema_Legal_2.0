using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExcelDataReader;
using System.Data;
using System.Reflection;
using System.Numerics;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using Sistema_Legal_2._0.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Entities;
using System.Configuration;
using Microsoft.Data.SqlClient;

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
            _configuration = configuration;
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





        [HttpGet("ObtenerPorNombre")]
        public IActionResult ObtenerDocumentosPorDescripcion(string nombre)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_cadenaSQL))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_obtener_documentos_por_nombre", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nombre", nombre);


                        SqlDataReader reader = cmd.ExecuteReader();
                        List<object> documentos = new List<object>();

                        while (reader.Read())
                        {
                            documentos.Add(new
                            {
                                Nombre = reader["nombre"].ToString(),
                                Ruta = reader["Ruta"].ToString()
                            });
                        }

                        return Ok(documentos);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }




        [HttpGet("archivos/{nombreArchivo}")]
        public IActionResult ObtenerArchivo(string nombreArchivo)
        {
            try
            {
                string rutaCarpeta = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";  // Ruta donde guardas los archivos
                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                if (!System.IO.File.Exists(rutaCompleta))
                {
                    return NotFound(new { mensaje = "Archivo no encontrado" });
                }

                string tipoMime = "application/octet-stream"; // Tipo MIME por defecto
                string extension = Path.GetExtension(rutaCompleta).ToLower();

                // Asignar tipos MIME comunes
                var tiposMime = new Dictionary<string, string>
        {
            { ".pdf", "application/pdf" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".jpg", "image/jpeg" },
            { ".png", "image/png" }
        };

                if (tiposMime.ContainsKey(extension))
                {
                    tipoMime = tiposMime[extension];
                }

                var archivoBytes = System.IO.File.ReadAllBytes(rutaCompleta);
                return File(archivoBytes, tipoMime);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpPost("Subir_Litigio_Con_Archivo")]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueCountLimit = int.MaxValue)]
        public async Task<IActionResult> SubirLitigioConArchivo([FromForm] LitigioConArchivo datos)
        {
            if (datos.Archivo == null || datos.Archivo.Length == 0)
                return BadRequest("No se recibió ningún archivo.");

            // Ruta base
            string rutaBase = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";

            // Crear ruta con estructura: SistemaLitigio\NombreDelActo\NombrePersonalizado
            string rutaActo = Path.Combine(rutaBase, datos.ltg_acto);
            string rutaFinal = Path.Combine(rutaActo, datos.NombreCarpeta);

            // Crear las carpetas si no existen
            if (!Directory.Exists(rutaFinal))
                Directory.CreateDirectory(rutaFinal);

            // Guardar el archivo en la ruta final
            string nombreArchivo = Path.GetFileName(datos.Archivo.FileName);
            string rutaArchivoCompleta = Path.Combine(rutaFinal, nombreArchivo);

            using (var stream = new FileStream(rutaArchivoCompleta, FileMode.Create))
            {
                await datos.Archivo.CopyToAsync(stream);
            }

            // Ruta relativa para guardar en la base de datos
            string rutaRelativa = Path.Combine(datos.ltg_acto, datos.NombreCarpeta, nombreArchivo);
            int idRutaArchivo = 0;

            using (SqlConnection connection = new SqlConnection(_cadenaSQL))
            {
                await connection.OpenAsync();

                // Guardar la ruta del archivo
                string insertRuta = "INSERT INTO Ruta_archivos (Ruta, Nombre, id_usuario) OUTPUT INSERTED.id_ruta VALUES (@Ruta, @Nombre, @id_usuario)";
                using (SqlCommand cmd = new SqlCommand(insertRuta, connection))
                {
                    cmd.Parameters.AddWithValue("@Ruta", rutaRelativa);
                    cmd.Parameters.AddWithValue("@Nombre", nombreArchivo);
                    cmd.Parameters.AddWithValue("@id_usuario", datos.id_usuario);
                    idRutaArchivo = (int)await cmd.ExecuteScalarAsync();
                }

                // Guardar el litigio
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
                    command.Parameters.AddWithValue("@ltg_Fecha_Audiencia", datos.ltg_Fecha_Audiencia);
                    command.Parameters.AddWithValue("@ltg_Fecha_Actualizacion", datos.ltg_Fecha_Actualizacion);
                    command.Parameters.AddWithValue("@id_Tribunal", datos.id_Tribunal);
                    command.Parameters.AddWithValue("@id_Sentencia", datos.id_Sentencia);
                    command.Parameters.AddWithValue("@id_usuario", datos.id_usuario);
                    command.Parameters.AddWithValue("@id_ruta", idRutaArchivo);
                    command.Parameters.AddWithValue("@id_Estatus", datos.id_Estatus);

                    await command.ExecuteNonQueryAsync(); 
                }
            }

            return Ok(new { mensaje = "Litigio y archivo subidos correctamente.", rutaRelativa });
        }

        [HttpPost("SubirArchivoExtra")]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueCountLimit = int.MaxValue)]
        public async Task<IActionResult> SubirArchivoExtra(IFormFile archivo, [FromForm] string nombreActo, [FromForm] string nombreSubCarpeta, [FromForm] int id_usuario)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                    return BadRequest("No se recibió ningún archivo.");

                // Ruta base
                string rutaBase = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";

                // Crear ruta completa: SistemaLitigio\NombreDelActo\NombreSubCarpeta
                string rutaActo = Path.Combine(rutaBase, nombreActo);
                string rutaFinal = Path.Combine(rutaActo, nombreSubCarpeta);

                // Crear carpetas si no existen
                if (!Directory.Exists(rutaFinal))
                    Directory.CreateDirectory(rutaFinal);

                // Guardar el archivo en esa ruta
                string nombreArchivo = Path.GetFileName(archivo.FileName);
                string rutaArchivoCompleta = Path.Combine(rutaFinal, nombreArchivo);

                using (var stream = new FileStream(rutaArchivoCompleta, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                // Ruta relativa para la base de datos
                string rutaRelativa = Path.Combine(nombreActo, nombreSubCarpeta, nombreArchivo);

                // Guardar en la base de datos
                using (SqlConnection connection = new SqlConnection(_cadenaSQL))
                {
                    string query = "INSERT INTO Ruta_archivos (Ruta, Nombre, id_usuario) VALUES (@Ruta, @Nombre, @id_usuario)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Ruta", rutaRelativa);
                        command.Parameters.AddWithValue("@Nombre", nombreArchivo);
                        command.Parameters.AddWithValue("@id_usuario", id_usuario);
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new { mensaje = "Archivo adicional subido correctamente.", rutaRelativa });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("ObtenerArchivosPorActo")]
        public IActionResult ObtenerArchivosPorActo([FromQuery] string nombreActo)
        {
            try
            {
                if (string.IsNullOrEmpty(nombreActo))
                    return BadRequest("Debe proporcionar el nombre del acto.");

                string rutaBase = @"C:\Users\mariancastillo\Desktop\SistemaLitigio";
                string rutaActo = Path.Combine(rutaBase, nombreActo);

                if (!Directory.Exists(rutaActo))
                    return NotFound("No existe ninguna carpeta para ese acto.");

                // Obtener todos los archivos incluyendo los de las subcarpetas
                var archivos = Directory.GetFiles(rutaActo, "*.*", SearchOption.AllDirectories)
                                        .Select(path => new
                                        {
                                            NombreArchivo = Path.GetFileName(path),
                                            RutaRelativa = Path.GetRelativePath(rutaBase, path),
                                            RutaCompleta = path
                                        })
                                        .ToList();

                return Ok(archivos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener: {{ex.Message}}");
                    }
        
        }
    
    }

}


