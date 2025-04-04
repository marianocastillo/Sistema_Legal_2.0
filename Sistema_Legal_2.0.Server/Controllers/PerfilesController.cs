using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using Sistema_Legal_2._0.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Models.Enums;
using Sistema_Legal_2._0.Server.Entities;

namespace Sistema_Legal_2._0.Server.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilesController : ControllerBase
    {
        private readonly PerfilesRepo perfilesRepo;
        private readonly db_silegContext db_silegContext;
        private readonly Logger _logger;

        /// <summary>
        /// Constructor de la clase PerfilesController.
        /// </summary>
        /// <param name=" db_silegContext">Contexto de la base de datos.</param>
        /// <param name="logger">Instancia del logger.</param>
        public PerfilesController(db_silegContext db_silegContext, Logger logger)
        {
            db_silegContext = db_silegContext;
            perfilesRepo = new PerfilesRepo(db_silegContext);
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los perfiles de usuario.
        /// </summary>
        /// <returns>Lista de perfiles de usuario.</returns>
        [HttpGet(Name = "GetPerfiles")]
        //[Authorize]
        [AllowAnonymous]
        public List<PerfilesModel> Get()
        {
            List<PerfilesModel> perfiles = perfilesRepo.Get().ToList();
            return perfiles;
        }

        /// <summary>
        /// Obtiene un perfil de usuario por su ID.
        /// </summary>
        /// <param name="idPerfil">ID del perfil.</param>
        /// <returns>Perfil de usuario.</returns>
        [HttpGet("{idPerfil}", Name = "GetPerfil")]
        //[Authorize]
        [AllowAnonymous]
        public PerfilesModel Get(int idPerfil)
        {
            PerfilesModel perfiles = perfilesRepo.Get(idPerfil);
            return perfiles;
        }

        /// <summary>
        /// Crea un nuevo perfil de usuario.
        /// </summary>
        /// <param name="perfilesModel">Datos del perfil de usuario.</param>
        /// <returns>Resultado de la operación.</returns>
        //[HttpPost(Name = "SavePerfil")]
        //[AuthorizeByPermission(PermisosEnum.Nuevo_Perfil)]
        //public OperationResult Post(PerfilesModel perfilesModel)
        //{
        //    try
        //    {
        //        if (perfilesRepo.Any(x => x.Nombre == perfilesModel.Nombre)) return new OperationResult(false, "Ya existe un perfil con este nombre.");
        //        var created = perfilesRepo.Add(perfilesModel);
        //        _logger.LogHttpRequest(perfilesModel);
        //        return new OperationResult(true, "Perfil creado exitosamente", created);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex);
        //        throw;
        //    }
        //}
        //[HttpPut(Name = "UpdatePerfil")]
        //[AuthorizeByPermission(PermisosEnum.Editar_Perfil)]
        //public OperationResult Put(PerfilesModel perfilModel)
        //{
        //    try
        //    {
        //        var perfil = perfilesRepo.Get(x => x.idPerfil == perfilModel.idPerfil).FirstOrDefault();

        //        if (perfil == null) return new OperationResult(false, "Este perfil no se ha encontrado");
        //        if (perfilesRepo.Any(x => x.Nombre == perfilModel.Nombre && x.idPerfil != perfilModel.idPerfil)) return new OperationResult(false, "Ya existe un perfil con este nombre.");

        //        perfilesRepo.Edit(perfilModel);
        //        _logger.LogHttpRequest(perfilModel);
        //        return new OperationResult(true, "Perfil editado exitosamente", perfil);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex);
        //        throw;
        //    }
        //}
        //[HttpDelete("{idPerfil}", Name = "DeletePerfil")]
        //[AuthorizeByPermission(PermisosEnum.Perfiles, PermisosEnum.Editar_Perfil)]
        //public OperationResult Delete(int idPerfil)
        //{
        //    try
        //    {
        //        perfilesRepo.Delete(idPerfil);
        //        _logger.LogHttpRequest(idPerfil);
        //        return new OperationResult(true, "Perfil eliminado exitosamente", idPerfil);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex);
        //        throw;
        //    }
        //}
        /// <summary>
        /// Obtiene los permisos de un perfil de usuario.
        /// </summary>
        /// <param name="idPerfil">ID del perfil.</param>
        /// <returns>Lista de permisos.</returns>
        [HttpGet("GetPermisos/{idPerfil?}", Name = "GetPermisos")]
        //[Authorize]
        [AllowAnonymous]
        public List<VistasModel> GetPermisos(int? idPerfil)
        {
            List<VistasModel> permisos = perfilesRepo.GetPermisos(idPerfil).ToList();
            return permisos;
        }

        /// <summary>
        /// Obtiene los usuarios asociados a un perfil de usuario.
        /// </summary>
        /// <param name="idPerfil">ID del perfil.</param>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet("GetUsuarios/{idPerfil}", Name = "GetUsuariosPerfil")]
        //[Authorize]
        [AllowAnonymous]
        public List<UsuariosModel> GetUsuarios(int idPerfil)
        {
            List<UsuariosModel> usuarios = perfilesRepo.GetUsuarios(idPerfil).ToList();
            return usuarios;
        }
    }
}