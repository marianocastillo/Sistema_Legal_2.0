using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sistema_Legal_2._0.Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Legal_2._0.Server.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly db_silegContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(db_silegContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Logins request)
        {
            if (request == null || string.IsNullOrEmpty(request.LogginUsuario) || string.IsNullOrEmpty(request.Contraseña))
            {
                return BadRequest(new { mensaje = "Usuario y contraseña son requeridos." });
            }

            var loggin = await _context.Logins
                .FirstOrDefaultAsync(u => u.LogginUsuario == request.LogginUsuario && u.Contraseña == request.Contraseña);

            if (loggin == null)
            {
                return Unauthorized(new { mensaje = "Credenciales inválidas." });
            }

            // Generar el token JWT
            var token = GenerarToken(loggin.LogginUsuario);

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso",
                token
            });
        }

        private string GenerarToken(string loggin)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loggin),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credenciales = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciales
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    }

    
    
