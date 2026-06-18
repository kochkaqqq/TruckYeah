using AdminService.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdminService.Application.Shared.Utils
{
    public static class JwtGenerator
    {
        public static string GenerateJwtToken(Admin admin)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, admin.Name),
                new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new(ClaimTypes.Role, "Admin")
            };

            var jwt = new JwtSecurityToken(
                issuer: "admin-service",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("your-super-secret-key-with-at-least-32-characters-long")),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
