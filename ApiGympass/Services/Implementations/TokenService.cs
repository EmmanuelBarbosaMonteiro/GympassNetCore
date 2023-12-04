using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGympass.Models;
using Microsoft.IdentityModel.Tokens;

namespace ApiGympass.Services.Implementations
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GenerateToken(User user)
        {
            Claim[] claims = new[]
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("user", user.UserName),
                new Claim("email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            
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