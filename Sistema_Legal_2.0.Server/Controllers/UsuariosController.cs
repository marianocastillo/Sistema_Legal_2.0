using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using Sistema_Legal_2._0.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Models.Enums;


namespace Sistema_Legal_2._0.Server.Controller
{
    [ApiController]
    [Route("api/[controller]")]
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
            _logger = logger;
            idUsuarioOnline = userAccessor.idUsuario;
        }

        [HttpGet(Name = "GetUsuarios")]
        [Authorize]
        public List<UsuariosModel> Get()
        {
            List<UsuariosModel> usuarios = usuariosRepo.Get().ToList();
            return usuarios;
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="idUsuario">ID del usuario.</param>
        /// <returns>Usuario encontrado.</returns>
        [HttpGet("{idUsuario}", Name = "GetUsuario")]
        [Authorize]
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
        [Authorize]
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
        public OperationResult Post(UsuariosModel usuariosModel)
        {
            try
            {
                if (usuariosRepo.Any(x => x.Nombres     == usuariosModel.NombreUsuario)) return new OperationResult(false, "Este usuario ya tiene acceso al sistema");
                    
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

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        /// <param name="usuariosModel">Datos del usuario a actualizar.</param>
        /// <returns>Resultado de la operacin.</returns>
        [HttpPut(Name = "UpdateUsuario")]        
        public OperationResult Put(UsuariosModel usuariosModel)
        {
            try
            {
                var usuario = usuariosRepo.Get(x => x.IdUsuario == usuariosModel.IdUsuario).FirstOrDefault();

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
        [AuthorizeByPermission(PermisosEnum.Usuarios, PermisosEnum.Editar_Usuario)]
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

    }
}