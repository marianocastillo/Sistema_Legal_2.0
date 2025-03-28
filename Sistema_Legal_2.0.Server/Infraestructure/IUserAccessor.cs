namespace Sistema_Legal_2._0.Server.Infraestructure;
public interface IUserAccessor
{
    int idUsuario { get; }
}
public class UserAccessor : IUserAccessor
{
    public int idUsuario { get; set; }
}