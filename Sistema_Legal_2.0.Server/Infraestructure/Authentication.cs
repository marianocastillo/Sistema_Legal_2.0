using Microsoft.IdentityModel.Tokens;
using Sistema_Legal_2._0.Server.Models.Enums;
using Sistema_Legal_2._0.Server.Providers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using Sistema_Legal_2._0.Server.Entities;



namespace Sistema_Legal_2._0.Server.Infraestructure;

public class Authentication
{
    private readonly ActiveDirectoryAuthenticationService _adAuthenticationService;
    private readonly db_silegContext db_SilegContext; 
    private readonly IConfiguration _configuration;
    public Authentication(ActiveDirectoryAuthenticationService adAuthenticationService, db_silegContext db_Registro, IConfiguration configuration)
    {
        _adAuthenticationService = adAuthenticationService;
        db_SilegContext = db_Registro;
        _configuration = configuration;
    }
    public OperationResult LogIn(Credentials credentials)
    {
        OperationResult logInResult;
        using (db_SilegContext)
        {
            UsuariosRepo ur = new UsuariosRepo(db_SilegContext);
            var usuario = ur.GetFirst(u => u.NombreUsuario.Equals(credentials.UserName, StringComparison.CurrentCultureIgnoreCase));

            ADRepository adRepository = new();

            var user = adRepository.GetUserData(credentials.UserName.ToLower());

            if (!_adAuthenticationService.ValidateCredentials(credentials.UserName.ToLower(), credentials.Password))
                return new OperationResult(false, "Usuario o contraseña inválidos", false);

            if (usuario == null) return new OperationResult(false, "El usuario " + credentials.UserName.ToLower() + " no tiene acceso al sistema", false);

            if (!usuario.Activo) return new OperationResult(false, "El usuario " + credentials.UserName.ToLower() + " está inactivo", false);
           

            string token = TokenGenerator(credentials.UserName, usuario.IdUsuario, usuario.IdPerfil);

            var data = new
            {
                usuario,
                
            };

            logInResult = new OperationResult(true, "Exito al iniciar sesión", data, token);
        }
        return logInResult;
    }
    private string TokenGenerator(string UserName, int idUsuario, int idPerfil)
    {
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        string? nombrePerfil = Enum.GetName(typeof(PerfilesEnum), idPerfil);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim("idUsuario", idUsuario.ToString()),
                new Claim(ClaimTypes.Role, nombrePerfil ?? ""),
                new Claim(JwtRegisteredClaimNames.Sub, UserName),
                new Claim(JwtRegisteredClaimNames.Email, UserName),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);

        return stringToken;
    }
}
