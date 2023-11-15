using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGympass.Models;
using Microsoft.IdentityModel.Tokens;

namespace ApiGympass.Services.Implementations
{
    public class TokenService
    {
        public string GenerateToken(User user)
        {
            Claim[] claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("user", user.UserName),
                new Claim("email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ksjnlfkjabsfivbasiubfiuabIABUIFBALIBF58489465484dkhg8784FVBSLDIUV"));
            
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    expires: DateTime.Now.AddHours(1),
                    claims: claims,
                    signingCredentials: signingCredentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}