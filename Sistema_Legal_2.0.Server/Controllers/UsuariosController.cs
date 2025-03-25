using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            _logger = logger;
            idUsuarioOnline = userAccessor.idUsuario;
        }

        // GET: UsuariosController/Create
        [HttpGet(Name = "GetUsuarios")]
        [Authorize]
        public List<UsuariosModel> Get()
        {
            List<UsuariosModel> usuarios = usuariosRepo.Get().ToList();
            return usuarios;
        }
        // GET: UsuariosController/Details/5
        [HttpGet("{idUsuario}", Name = "GetUsuario")]
        [Authorize]
        public UsuariosModel Get(int idUsuario)
        {
            UsuariosModel usuarios = usuariosRepo.Get(idUsuario);
            return usuarios;
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
