using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Models.Enums;
using Sistema_Legal_2._0.Server.Repositories;

namespace Sistema_Legal_2._0.Server.Controllers
{
    [ApiController]
    [Route("api/[Cotroller]")]
    public class UsuariosController : Controller
    {
        private readonly UsuariosRepo usuariosRepo;
        private readonly PerfilesRepo perfilesRepo;
        private readonly db_silegContext _db_SilegContext;
        private readonly Logger _logger;
        private int idUsuarioOnline;



        public UsuariosController(db_silegContext db_SilegContext, IdentityUserAccessor userAccessor, Logger logger ) 
        {
            _db_SilegContext = db_SilegContext;
            usuariosRepo = new UsuariosRepo(db_SilegContext);
            perfilesRepo = new PerfilesRepo(db_SilegContext);
=======
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using Sistema_Legal_2._0.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Models.Enums;
using Sistema_Legal_2._0.Server.Entities;
using Microsoft.EntityFrameworkCore;

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
        private int idUsuarioOnline;
        public UsuariosController(db_silegContext db_silegContext_, IUserAccessor userAccessor, Logger logger)
        {
            _db_silegContext = db_silegContext_;
            usuariosRepo = new UsuariosRepo(db_silegContext_);
            perfilesRepo = new PerfilesRepo(db_silegContext_);
>>>>>>> Developer-Fronk
            _logger = logger;
            idUsuarioOnline = userAccessor.idUsuario;
        }

<<<<<<< HEAD
        // GET: UsuariosController/Create
        [HttpGet(Name = "GetUsuarios")]
        [Authorize]
        public List<UsuariosModel> Get()
=======
        [HttpGet("UsuariosConPerfil")]
        public IActionResult GetUsuariosConPerfil()
>>>>>>> Developer-Fronk
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
<<<<<<< HEAD
        // GET: UsuariosController/Details/5
=======



        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="idUsuario">ID del usuario.</param>
        /// <returns>Usuario encontrado.</returns>
>>>>>>> Developer-Fronk
        [HttpGet("{idUsuario}", Name = "GetUsuario")]
        [AllowAnonymous]
        public UsuariosModel Get(int idUsuario)
        {
            UsuariosModel usuarios = usuariosRepo.Get(idUsuario);
            return usuarios;
        }

<<<<<<< HEAD
=======
        /// <summary>
        /// Obtiene un usuario de Active Directory por su nombre de usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario.</param>
        /// <returns>Usuario de Active Directory encontrado.</returns>
>>>>>>> Developer-Fronk
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
<<<<<<< HEAD








        // POST: UsuariosController/Create
        [HttpPost(Name = "SaveUsuario")]
        public OperationResult Post(UsuariosModel usuariosModel)
        {
            try
            {
                if (usuariosRepo.Any(x => x.Usuario == usuariosModel.NombreUsuario)) return new OperationResult(false, "Este usuario ya tiene acceso al sistema");

                usuariosModel.FechaCrea = DateTime.Now;

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


        [HttpPut(Name = "UpdateUsuario")]
        public OperationResult Put(UsuariosModel usuariosModel)
        {
            try
            {
                var usuario = usuariosRepo.Get(x => x.idUsuario == usuariosModel.idUsuario).FirstOrDefault();

                if (usuario == null) return new OperationResult(false, "El usuario no se ha encontrado");
                usuario.idPerfil = usuariosModel.idPerfil;
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


        [HttpDelete("{idUsuario}", Name = "DeleteUsuario")]
        [AuthorizeByPermission(PermisosEnum.Usuarios, PermisosEnum.Editar_Usuario)]
        public OperationResult Delete(int idUsuario)
=======









        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="usuariosModel">Datos del usuario a crear.</param>
        /// <returns>Resultado de la operacin.</returns>
        [HttpPost(Name = "SaveUsuario")]
        [AllowAnonymous]
        public OperationResult Post(UsuariosModel usuariosModel)
>>>>>>> Developer-Fronk
        {
                       try
            {
<<<<<<< HEAD
                usuariosRepo.Delete(idUsuario);
                _logger.LogHttpRequest(idUsuario);
                return new OperationResult(true, "Usuario eliminado exitosamente", idUsuario);
=======
                if (usuariosRepo.Any(x => x.nombreUsuario == usuariosModel.NombreUsuario))
                    return new OperationResult(false, "Este usuario ya tiene acceso al sistema");

                usuariosModel.FechaCreacion = DateTime.Now;
                var created = usuariosRepo.Add(usuariosModel);
                _logger.LogHttpRequest(usuariosModel);
                return new OperationResult(true, "Usuario creado exitosamente", created);
>>>>>>> Developer-Fronk
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
<<<<<<< HEAD
=======
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
>>>>>>> Developer-Fronk
                throw;
            }
        }
    }
}
