using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using  RestApiProject.Models;
using System.Text;

namespace RestApiProject.Services
{
    public class JWTService
    {

        private readonly string _secret;
        private readonly string _issuer;

        public JWTService(IConfiguration config)
        {
            _secret = config["Jwt:Key"] ?? "super_secret_key_123!";
            _issuer = config["Jwt:Issuer"] ?? "BookCatalogAPI";
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);//for hashing


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // 👈 add userId
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
           
        };

            var token = new JwtSecurityToken(
                _issuer,
                _issuer,
                claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }





}
