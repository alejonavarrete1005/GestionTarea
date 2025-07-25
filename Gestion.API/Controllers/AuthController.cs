using Dapper;
using GestionTareas.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionTareas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IDbConnection _db;      // Inyectado desde el contenedor (AddScoped<IDbConnection>)
        private readonly IConfiguration _config; // Para leer Jwt:Key y Jwt:Issuer

        public AuthController(IDbConnection db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // ---------- REGISTRO ----------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            const string sqlCheck = "SELECT 1 FROM Usuarios WHERE Correo = @Correo";
            var correoOcupado = await _db.ExecuteScalarAsync<bool>(sqlCheck, new { dto.Correo });
            if (correoOcupado) return BadRequest("El correo electrónico ya está registrado.");

            // Hash de la contraseña
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Clave);

            const string sqlInsert =
                @"INSERT INTO Usuarios (NombreCompleto, Correo, ClaveHash, Rol)
                  VALUES (@NombreCompleto, @Correo, @ClaveHash, @Rol);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var id = await _db.ExecuteScalarAsync<int>(sqlInsert, new
            {
                dto.NombreCompleto,
                dto.Correo,
                ClaveHash = hash,
                Rol = "Usuario"
            });

            var token = GenerarJwt(id, dto.Correo, "Usuario");

            return Ok(new AuthResponseDto
            {
                Token = token,
                Usuario = new Usuario
                {
                    Id = id,
                    NombreCompleto = dto.NombreCompleto,
                    Correo = dto.Correo,
                    Rol = "Usuario"
                }
            });
        }

        // ---------- LOGIN ----------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            const string sql = "SELECT TOP 1 * FROM Usuarios WHERE Correo = @Correo";
            var user = await _db.QueryFirstOrDefaultAsync<Usuario>(sql, new { dto.Correo });

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Clave, user.Clave))
                return Unauthorized("Credenciales incorrectas.");

            var token = GenerarJwt(user.Id, user.Correo, user.Rol);

            return Ok(new AuthResponseDto { Token = token, Usuario = user });
        }

        // ---------- JWT ----------
        private string GenerarJwt(int id, string correo, string rol)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, correo),
                new Claim(ClaimTypes.Role, rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ---------- DTOs ----------
        public record RegisterRequestDto(string NombreCompleto, string Correo, string Clave);
        public record LoginRequestDto(string Correo, string Clave);
        public record AuthResponseDto
        {
            public string Token { get; init; } = string.Empty;
            public Usuario Usuario { get; init; } = default!;
        }
    }
}
