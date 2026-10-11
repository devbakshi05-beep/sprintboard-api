using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KanbanBackend.Models;
using Microsoft.IdentityModel.Tokens;

namespace KanbanBackend.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        // Ye method login/register ke baad ek JWT token banata hai
        // Ye token frontend localStorage me store karega aur har request ke saath bhejega
        public string GenerateToken(ApplicationUser user)
        {
            var jwtSecret = _config["Jwt:Secret"]
                ?? throw new InvalidOperationException("JWT secret configured nahi hai appsettings.json me");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // 7 din baad token expire hoga, phir dobara login
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
