using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using Sistema_Legal_2._0.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Models.Enums;
using Sistema_Legal_2._0.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Sistema_Legal_2._0.Server.Controller
{
    [ApiController]
    [Route("api/Usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosRepo usuariosRepo;
        private readonly PerfilesRepo perfilesRepo;
        private readonly db_silegContext _db_silegContext;
        private readonly Logger _logger;
        private readonly IConfiguration _configuration;
        private int idUsuarioOnline;
        public UsuariosController(db_silegContext db_silegContext_, IConfiguration configuration, IUserAccessor userAccessor, Logger logger)
        {
            _db_silegContext = db_silegContext_;
            usuariosRepo = new UsuariosRepo(db_silegContext_);
            perfilesRepo = new PerfilesRepo(db_silegContext_);
            _logger = logger;
            _configuration = configuration;
            idUsuarioOnline = userAccessor.idUsuario;
        }

        [HttpGet("UsuariosConPerfil")]
        public IActionResult GetUsuariosConPerfil()
        {
            try
            {
                // Esto hace el JOIN con Perfiles y trae nombrePerfil incluido
                var usuarios = usuariosRepo.GetWithPerfil(u => true);

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los usuarios.", error = ex.Message });
            }
        }



        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="idUsuario">ID del usuario.</param>
        /// <returns>Usuario encontrado.</returns>
        [HttpGet("{idUsuario}", Name = "GetUsuario")]
        [AllowAnonymous]
        public UsuariosModel Get(int idUsuario)
        {
            UsuariosModel usuario = usuariosRepo.Get(idUsuario);
            return usuario;
        }

        /// <summary>
        /// Obtiene un usuario de Active Directory por su nombre de usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario.</param>
        /// <returns>Usuario de Active Directory encontrado.</returns>
        [HttpGet("GetADUser/{nombreUsuario}", Name = "GetADUser")]
        [AllowAnonymous]
        public UsuariosModel? GetADUser(string nombreUsuario)
        {
            AdUser? adUser = usuariosRepo.GetADUser(nombreUsuario);

            if (adUser == null) return null;

            UsuariosModel usuario = new UsuariosModel()
            {

                Nombres = adUser.firstName.ToString(),
                Apellidos = adUser.lastName.ToString(),
                NombreUsuario = nombreUsuario.ToLower(),
                IdPerfil = perfilesRepo.GetPerfilDefault(),
                Activo = true
            };
            return usuario;
        }









        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="usuariosModel">Datos del usuario a crear.</param>
        /// <returns>Resultado de la operacin.</returns>
        [HttpPost(Name = "SaveUsuario")]
        [AllowAnonymous]
        public OperationResult Post(UsuariosModel usuariosModel)
        {
            try
            {
                if (usuariosRepo.Any(x => x.nombreUsuario == usuariosModel.NombreUsuario))
                    return new OperationResult(false, "Este usuario ya tiene acceso al sistema");

                usuariosModel.FechaCreacion = DateTime.Now;
                var created = usuariosRepo.Add(usuariosModel);
                _logger.LogHttpRequest(usuariosModel);
                return new OperationResult(true, "Usuario creado exitosamente", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }









        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        /// <param name="usuariosModel">Datos del usuario a actualizar.</param>
        /// <returns>Resultado de la operacin.</returns>


        [HttpPut(Name = "UpdateUsuario")]
        [AllowAnonymous]
        public OperationResult Put(UsuariosModel usuariosModel)
        {
            try
            {
                var usuario = usuariosRepo.Get(x => x.idUsuario == usuariosModel.IdUsuario).FirstOrDefault();

                if (usuario == null) return new OperationResult(false, "El usuario no se ha encontrado");
                usuario.IdPerfil = usuariosModel.IdPerfil;
                usuario.Activo = usuariosModel.Activo;

                usuariosRepo.Edit(usuario);
                _logger.LogHttpRequest(usuariosModel);
                return new OperationResult(true, "Usuario modificado exitosamente", usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }



        /// <summary>
        /// Elimina un perfil de usuario por su ID.
        /// </summary>
        /// <param name="idUsuario">ID del perfil.</param>
        /// <returns>Resultado de la operacin.</returns>
        [HttpDelete("{idUsuario}", Name = "DeleteUsuario")]
        [AllowAnonymous]
        //[AuthorizeByPermission(PermisosEnum.Usuarios, PermisosEnum.Editar_Usuario)]
        public OperationResult Delete(int idUsuario)
        {
            try
            {
                usuariosRepo.Delete(idUsuario);
                _logger.LogHttpRequest(idUsuario);
                return new OperationResult(true, "Usuario eliminado exitosamente", idUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }

        [HttpPost("Asignar-Litigio")]
        [AllowAnonymous]
        public IActionResult Asignar([FromBody] Asignaciones_Ltg model)
        {
            if (model == null || model.IdUsuario <= 0 || model.IdLtg <= 0)
                return BadRequest("Datos inválidos.");

            string connectionString = _configuration.GetConnectionString("Sistema_Legal");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Asignaciones_Litigios (IdUsuario, Id_Ltg) VALUES (@idUsuario, @id_Ltg)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idUsuario", model.IdUsuario);
                    cmd.Parameters.AddWithValue("@id_Ltg", model.IdLtg);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (rows > 0)
                        return Ok("Asociación guardada correctamente.");
                    else
                        return StatusCode(500, "No se pudo guardar.");
                }
            }
        }

        [HttpGet("Asignados/{idLtg}")]
        public IActionResult GetAbogadosAsignadosConCantidad(int idLtg)
        {
            var lista = new List<object>();
            string connectionString = _configuration.GetConnectionString("Sistema_Legal");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                u.idUsuario,
                u.nombres,
                u.apellidos,
                p.nombre AS perfil,
                (
                    SELECT COUNT(*) 
                    FROM Asignaciones_Litigios 
                    WHERE IdUsuario = u.idUsuario
                ) AS cantidadAsignaciones
            FROM Asignaciones_Litigios a
            INNER JOIN Usuarios u ON u.idUsuario = a.IdUsuario
            INNER JOIN Perfiles p ON u.idPerfil = p.idPerfil
            WHERE a.Id_Ltg = @idLtg";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idLtg", idLtg);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new
                            {
                                idUsuario = reader["idUsuario"],
                                nombres = reader["nombres"].ToString().Trim(),
                                apellidos = reader["apellidos"].ToString().Trim(),
                                perfil = reader["perfil"].ToString().Trim(),
                                cantidadAsignaciones = Convert.ToInt32(reader["cantidadAsignaciones"])
                            });
                        }
                    }

                    conn.Close();
                }
            }

            return Ok(lista);
        }

        [HttpGet("AbogadosConAsignaciones")]
        public IActionResult GetAbogadosConCantidadAsignaciones()
        {
            var lista = new List<object>();
            string connectionString = _configuration.GetConnectionString("Sistema_Legal");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT u.idUsuario, u.nombres, u.apellidos, p.nombre AS nombrePerfil,
                   COUNT(a.Id_Ltg) AS cantidadAsignaciones
            FROM Usuarios u
            INNER JOIN Perfiles p ON u.idPerfil = p.idPerfil
            LEFT JOIN Asignaciones_Litigios a ON a.IdUsuario = u.idUsuario
            WHERE u.idPerfil = 4
            GROUP BY u.idUsuario, u.nombres, u.apellidos, p.nombre";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new
                            {
                                idUsuario = reader.GetInt32(0),
                                nombres = reader["nombres"].ToString(),
                                apellidos = reader["apellidos"].ToString(),
                                nombrePerfil = reader["nombrePerfil"].ToString(),
                                cantidadAsignaciones = reader.GetInt32(reader.GetOrdinal("cantidadAsignaciones"))
                            });
                        }
                    }
                }
            }

            return Ok(lista);
        }





        [HttpDelete("EliminarAsignacion")]
        public IActionResult EliminarAsignacion([FromQuery] int idUsuario, [FromQuery] int idLtg)
        {
            if (idUsuario <= 0 || idLtg <= 0)
                return BadRequest("Parámetros inválidos.");

            try
            {
                string connectionString = _configuration.GetConnectionString("Sistema_Legal");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Asignaciones_Litigios WHERE IdUsuario = @idUsuario AND Id_Ltg = @idLtg";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                        cmd.Parameters.AddWithValue("@idLtg", idLtg);

                        conn.Open();
                        int filas = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (filas > 0)
                            return Ok("Asignación eliminada correctamente.");
                        else
                            return NotFound("No se encontró la asignación.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar asignación.", error = ex.Message });
            }
        }


    }
}