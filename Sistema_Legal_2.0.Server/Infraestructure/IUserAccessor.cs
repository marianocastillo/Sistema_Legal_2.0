public interface IUserAccessor
{
    int idUsuario { get; }
}
public class UserAccessor : IUserAccessor
{
    public int idUsuario { get; set; }
}