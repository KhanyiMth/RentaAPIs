using Microsoft.IdentityModel.Tokens;
using RentaAPIs.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RentaAPIs.Services
{
    public class jwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public jwtService (IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim (JwtRegisteredClaimNames.Sub, user.Id),
                new Claim (JwtRegisteredClaimNames.Email, user.Email!),
                new Claim (ClaimTypes.NameIdentifier, user.Id)
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpirationMinutes"]!));

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
