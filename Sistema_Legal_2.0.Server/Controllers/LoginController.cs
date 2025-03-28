using Microsoft.AspNetCore.Mvc;

using Microsoft.IdentityModel.Tokens;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using Sistema_Legal_2._0.Server.Infraestructure;
using Microsoft.AspNetCore.Authorization;

namespace Sistema_Legal_2._0.Server.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly db_silegContext _context;
        private readonly Authentication _authentication;
        private readonly IConfiguration _configuration;
       
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Logger _logger;
        private readonly UsuariosRepo _usuariosRepo;
        private readonly PerfilesRepo _perfilesRepo;
        private int idUsuarioOnline;

        public AuthController(db_silegContext db_silegContext, IUserAccessor userAccessor, Authentication authentication, IHttpContextAccessor httpContextAccessor, Logger logger)
        {
          
            _authentication = authentication;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _perfilesRepo = new PerfilesRepo(db_silegContext);
            _usuariosRepo = new UsuariosRepo(db_silegContext);
            idUsuarioOnline = userAccessor.idUsuario;
        }

        [HttpGet(Name = "GetUserData")]
        [Authorize]
        public object GetUserData()
        {
            UsuariosModel usuario = _usuariosRepo.Get(x => x.IdUsuario == idUsuarioOnline).FirstOrDefault();
            List<VistasModel> vistas = [.. _perfilesRepo.GetPermisos(Convert.ToInt32(usuario.IdPerfil)).Where(v => v.Permiso)];
            return new
            {    
                usuario = usuario,
                vistas = vistas
            };
        }

        [HttpPost(Name = "LogIn")]
        [AllowAnonymous]
        public OperationResult LogIn(Credentials credentials)
        {
            try
            {
                if (credentials.UserName.Contains("@contraloria.gob.do"))
                {
                    credentials.UserName = credentials.UserName.Replace("@contraloria.gob.do", "");
                }

                OperationResult result = _authentication.LogIn(credentials);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new OperationResult(false, ex.Message);
            }

        }


    }
}
    

    
    
