using System.Security.Claims;

namespace Sistema_Legal_2._0.Server.Infraestructure;

public class OnlineUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public OnlineUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserID()
    {
        var User = _httpContextAccessor.HttpContext?.User;
        return Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "idUsuario")?.Value);
    }
}
