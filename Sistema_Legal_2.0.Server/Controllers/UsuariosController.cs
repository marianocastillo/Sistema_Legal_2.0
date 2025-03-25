using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//using EjecucionPresupuestal.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2.Server.Repositories;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Sistema_Legal_2._0.Server.Models.Enums;
using Sistema_Legal_2.Server.Controllers;




namespace Sistema_Legal_2._0.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosRepo usuariosRepo;
        private readonly PerfilesRepo perfilesRepo;
        private readonly db_silegContext _db_Sistema_Legal_2;
        private readonly ILogger<UsuariosController> _logger;
        private int idUsuarioOnline;
        public UsuariosController(db_silegContext db_silegContext, IUserAccessor userAccessor, Logger logger)
        {
            _db_Sistema_Legal_2 = db_silegContext;
            usuariosRepo = new UsuariosRepo(db_silegContext);
            perfilesRepo = new PerfilesRepo(db_silegContext);
            _logger = (ILogger<UsuariosController>)logger;
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
                idPerfil = perfilesRepo.GetPerfilDefault(),
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
        public Models.OperationResult Post(UsuariosModel usuariosModel)
        {
            try
            {
                if (usuariosRepo.Any(x => x.Usuario == usuariosModel.NombreUsuario)) return new Models.OperationResult(false, "Este usuario ya tiene acceso al sistema");

                usuariosModel.FechaCrea = DateTime.Now;

                var created = usuariosRepo.Add(usuariosModel);

                return new Models.OperationResult(true, "Usuario creado exitosamente", created);
            }
            catch (Exception)
            {
                _logger.LogError("Este es un error");
                throw;
            }
        }

        public Models.OperationResult Put(UsuariosModel usuariosModel, ILogger<UsuariosController> logger)
        {
            return Put(usuariosModel, _logger);
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        /// <param name="usuariosModel">Datos del usuario a actualizar.</param>
        /// <returns>Resultado de la operacin.</returns>
        /// 

        [HttpPut(Name = "UpdateUsuario")]
        public Models.OperationResult Put(UsuariosModel usuariosModel, Logger _logger)
        {

            try
            {
                var usuario = usuariosRepo.Get(x => x.idUsuario == usuariosModel.idUsuario).FirstOrDefault();

                if (usuario == null) return new Models.OperationResult(false, "El usuario no se ha encontrado");
                usuario.idPerfil = usuariosModel.idPerfil;
                usuario.Activo = usuariosModel.Activo;

                usuariosRepo.Edit(usuario);

                return new Models.OperationResult(true, "Usuario modificado exitosamente", usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError("Este es un error");
                throw;
            }

        }

        [HttpDelete("{idUsuario}", Name = "DeleteUsuario")]
        [AuthorizeByPermission(PermisosEnum.Usuarios, PermisosEnum.Editar_Usuario)]
        private Models.OperationResult Delete(int idUsuario, Logger _logger)
        {
            try
            {
                usuariosRepo.Delete(idUsuario);

                return new Models.OperationResult(true, "Usuario eliminado exitosamente", idUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError("Este es un error");


                throw;
            }
        }
    }
}