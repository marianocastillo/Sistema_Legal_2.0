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
        [HttpGet("GetPerfiles")]
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
        [HttpPost("SavePerfil")]
        [AllowAnonymous]
        // [AuthorizeByPermission(PermisosEnum.Nuevo_Perfil)]
        public OperationResult Post([FromBody] PerfilesModel perfilesModel)
        {
            try
            {
                if (perfilesRepo.Any(x => x.Nombre == perfilesModel.Nombre))
                    return new OperationResult(false, "Ya existe un perfil con este nombre.");

                var created = perfilesRepo.Add(perfilesModel);

                // DTO limpio para evitar ciclos de referencia en la serialización
                var dto = new
                {
                    idPerfil = created.idPerfil,
                    nombre = created.Nombre,
                    descripcion = created.Descripcion,
                    porDefecto = created.porDefecto ?? false,
                    cantPermisos = created.perfilesVistas?.Count ?? 0
                };

                _logger.LogHttpRequest(perfilesModel);
                return new OperationResult(true, "Perfil creado exitosamente", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }


[HttpPut("UpdatePerfil")]
[AllowAnonymous]
//[AuthorizeByPermission(PermisosEnum.Editar_Perfil)]
public OperationResult Put([FromBody] PerfilesModel perfilModel)
{
    try
    {
        var perfil = perfilesRepo.Get(x => x.idPerfil == perfilModel.idPerfil).FirstOrDefault();

        if (perfil == null)
            return new OperationResult(false, "Este perfil no se ha encontrado");

        if (perfilesRepo.Any(x => x.Nombre == perfilModel.Nombre && x.idPerfil != perfilModel.idPerfil))
            return new OperationResult(false, "Ya existe un perfil con este nombre.");

        perfilesRepo.Edit(perfilModel);
        _logger.LogHttpRequest(perfilModel);

                var perfilActualizado = perfilesRepo.Get(x => x.idPerfil == perfilModel.idPerfil).FirstOrDefault();

                var vistasIds = perfilModel.Vistas?
                    .Where(v => v.Permiso)
                    .Select(v => v.idVista)
                    .ToList() ?? new List<int>();

                var dto = new PerfilDto
                {
                    IdPerfil = perfilActualizado.idPerfil,
                    Nombre = perfilActualizado.Nombre,
                    Descripcion = perfilActualizado.Descripcion,
                    PorDefecto = perfilActualizado.porDefecto ?? false,
                    CantPermisos = vistasIds.Count,
                    Vistas = vistasIds
                };

                return new OperationResult(true, "Perfil editado exitosamente", dto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex);
        throw;
    }
}





        [HttpDelete("DeletePerfil/{idPerfil}")]
        [AllowAnonymous]
        // [AuthorizeByPermission(PermisosEnum.Perfiles, PermisosEnum.Editar_Perfil)]
        public OperationResult Delete(int idPerfil)
        {
            try
            {
                if (!perfilesRepo.CanDelete(idPerfil))
                    return new OperationResult(false, "No puedes eliminar un perfil con usuarios asignados.");
                _logger.LogHttpRequest(idPerfil);
                return new OperationResult(true, "Perfil eliminado exitosamente", idPerfil);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }








        /// <summary>
        /// Obtiene los permisos de un perfil de usuario.
        /// </summary>
        /// <param name="idPerfil">ID del perfil.</param>
        /// <returns>Lista de permisos.</returns>
        [HttpGet("GetPermisos/{idPerfil?}")]
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
        [HttpGet("GetUsuariosPerfiles/{idPerfil}")]
        //[Authorize]
        [AllowAnonymous]
        public List<UsuariosModel> GetUsuarios(int idPerfil)
        {
            List<UsuariosModel> usuarios = perfilesRepo.GetUsuarios(idPerfil).ToList();
            return usuarios;
        }

        /// <summary>
        /// Obtiene todas las vistas disponibles del sistema.
        /// </summary>
        /// <returns>Lista de vistas.</returns>
        [HttpGet("GetVistas")]
        [AllowAnonymous] // o [Authorize] si lo prefieres
        public List<VistasModel> GetVistas()
        {
            return perfilesRepo.GetVistas().ToList();
        }

    }
}