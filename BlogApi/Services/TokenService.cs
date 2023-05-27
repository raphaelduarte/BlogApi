using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Services
{
    public class TokenService
    {
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "Raphael Duarte"),   //User.Identity.Name
                    new Claim(ClaimTypes.Role, "admin"),             //User.IsInRole
                    new Claim(ClaimTypes.Role, "user")             //User.IsInRole
                }),
                Expires = DateTime.UtcNow.AddDays(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);


            return tokenHandler.WriteToken(token);
        }
    }
}
