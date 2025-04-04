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
        private  IConfiguration configuration;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;
        private readonly db_silegContext _db_silegContext;
        public FilesController(IConfiguration config, db_silegContext db_SilegContext )
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


        [HttpPost("Subir_Litigio")]

        public async Task<IActionResult> CrearLitigio([FromBody] Litigio litigio)
        {
            using (SqlConnection connection = new SqlConnection(_cadenaSQL))
            {
                using (SqlCommand command = new SqlCommand("sp_CrearLitigio", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ltg_acto", litigio.ltg_acto);
                    command.Parameters.AddWithValue("@ltg_Fecha_Acto", litigio.ltg_Fecha_Acto);
                    command.Parameters.AddWithValue("@id_Tipo_Demanda", litigio.id_Tipo_Demanda);
                    command.Parameters.AddWithValue("@ltg_Cedula_Demandante", litigio.ltg_Cedula_Demandante);
                    command.Parameters.AddWithValue("@ltg_Nacionalidad", litigio.ltg_Nacionalidad);
                    command.Parameters.AddWithValue("@ltg_Demandante", litigio.ltg_Demandante);
                    command.Parameters.AddWithValue("@ltg_Tipo_Demandante", litigio.ltg_Tipo_Demandante);
                    command.Parameters.AddWithValue("@ltg_Cedula_Representante", litigio.ltg_Cedula_Representante);
                    command.Parameters.AddWithValue("@ltg_Nombre_Representante", litigio.ltg_Nombre_Representante);
                    command.Parameters.AddWithValue("@ltg_Fecha_Audiencia", litigio.ltg_Fecha_Audiencia);
                    command.Parameters.AddWithValue("@ltg_Doc_Demandante", litigio.ltg_Doc_Demandante);
                    command.Parameters.AddWithValue("@ltg_Fecha_Actualizacion", litigio.ltg_Fecha_Actualizacion);
                    command.Parameters.AddWithValue("@id_Tribunal", litigio.id_Tribunal);
                    command.Parameters.AddWithValue("@id_Sentencia", litigio.id_Sentencia);
                    command.Parameters.AddWithValue("@id_usuario", litigio.id_usuario);
                    command.Parameters.AddWithValue("@id_ruta", litigio.id_ruta);
                    command.Parameters.AddWithValue("@id_Estatus", litigio.id_Estatus);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
            return CreatedAtAction(nameof(CrearLitigio), litigio);
        }







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
                string rutaCarpeta = @"C:\ProgramasVisual\Archivos\"; // Ruta donde guardas los archivos
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







    }



}

