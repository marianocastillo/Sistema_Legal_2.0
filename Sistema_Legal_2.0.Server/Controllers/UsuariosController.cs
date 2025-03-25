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

    public static class LoggerExtensions
    {
        public static void LogError(this Logger logger, string mensaje)
        {
            // Asumiendo que Logger tiene un método para escribir en logs
            // Usa un método disponible de Logger si tiene uno similar
            logger.GetType().GetMethod("Log")?.Invoke(logger, new object[] { mensaje });
        }
    }


    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosRepo usuariosRepo;
        private readonly PerfilesRepo perfilesRepo;
        private readonly db_silegContext _db_Sistema_Legal_2;
        private readonly ILogger<UsuariosController> _logger;
        private int idUsuarioOnline;

        private readonly Logger _ogger;

        public UsuariosController(Logger logger)
        {
            _ogger = logger;
        }

        public UsuariosController(db_silegContext db_silegContext, IUserAccessor userAccessor, Logger logger)
        {
            _db_Sistema_Legal_2 = db_silegContext;
            usuariosRepo = new UsuariosRepo(db_silegContext);
            perfilesRepo = new PerfilesRepo(db_silegContext);
            _logger = (ILogger<UsuariosController>)logger;
            idUsuarioOnline = userAccessor.idUsuario;
        }

        [HttpGet(Name = "GetUsuarios")]
        public List<UsuariosModel> Get()
        {
            List<UsuariosModel> usuarios = usuariosRepo.Get().ToList();
            return usuarios;
        }

        [HttpGet("{idUsuario}", Name = "GetUsuario")]
        [Authorize]
        public UsuariosModel Get(int idUsuario)
        {
            UsuariosModel usuario = usuariosRepo.Get(idUsuario);
            return usuario;
        }

       
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
        
       [HttpPost("Guardar Usuario", Name = "SaveUsuario")]
        [Authorize]
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





        [HttpPut("ActualizarUsuario",Name = "UpdateUsuario")]
        [Authorize]
        public Models.OperationResult Put([FromBody] UsuariosModel usuariosModel)

        {
            try
            {
                var usuario = usuariosRepo.Get(x => x.idUsuario == usuariosModel.idUsuario).FirstOrDefault();

                if (usuario == null)
                    return new Models.OperationResult(false, "El usuario no se ha encontrado");

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


        /// <summary>
        ///     
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="_ogger"></param>
        /// <returns></returns>


        [HttpDelete("EliminarUsuario/{idUsuario}", Name = "DeleteUsuario")]
        [AuthorizeByPermission(PermisosEnum.Usuarios, PermisosEnum.Editar_Usuario)]
        public Models.OperationResult Delete(int idUsuario)
        {
            try
            {
                // Eliminar el usuario usando el repositorio
                usuariosRepo.Delete(idUsuario);

                // Retornar el resultado de la operación
                return new Models.OperationResult(true, "Usuario eliminado exitosamente", idUsuario);
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se captura la excepción y se registra el error
                _logger.LogError($"Error al eliminar el usuario: {ex.Message}");
                throw;
            }
        }

    }
}