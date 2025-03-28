using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sistema_Legal_2._0.Server.Models;




namespace Sistema_Legal_2._0.Server.Infraestructure
{
    public class Logger
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int OnlineUserID;
        private readonly IUserAccessor _userAccessor;
        private readonly db_silegContext _db_SilegContext;
        public Logger(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, IUserAccessor userAccessor, IConfiguration configuration, db_silegContext dbSilegContext)
        {
            var connectionString = configuration.GetConnectionString("Sistema_Legal");

            var contextOptions = new DbContextOptionsBuilder<db_silegContext>()
                                    .UseSqlServer(connectionString)
                                    .Options;

            _userAccessor = userAccessor;

            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            OnlineUserID = userAccessor.idUsuario;
            _db_SilegContext = new db_silegContext(contextOptions);
        }
        public void LogHttpRequest(object data)
        {
            LogActividad log = new()
            {
                Url = _httpContextAccessor.HttpContext.Request.Path,
                IdUsuario = OnlineUserID,
                Metodo = _httpContextAccessor.HttpContext.Request.Method,
                Fecha = DateTime.Now,
                Data = data == null ? String.Empty : JsonConvert.SerializeObject(data)
            };

            _db_SilegContext.Set<LogActividad>().Add(log);
            _db_SilegContext.SaveChanges();
        }
        public void LogError(Exception ex)
        {
            LogError log = new()
            {
                IdUsuario = OnlineUserID,
                Fecha = DateTime.Now,
                Mensaje = ex.Message,
                StackTrace = ex.StackTrace,
                Origen = ex.Source,
                Tipo = "Excepción de Sistema"
            };

            _db_SilegContext.Set<LogError>().Add(log);
            _db_SilegContext.SaveChanges();
        }

        public void LogHttpRequest(LogActividad log)
        {
            _db_SilegContext.Set<LogActividad>().Add(log);
            _db_SilegContext.SaveChanges();
        }
    }


}
