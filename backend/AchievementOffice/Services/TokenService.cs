using AchievementOffice.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AchievementOffice.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var issuer = _configuration["JwtConf:Issuer"];
            var audience = _configuration["JwtConf:Audience"];
            var signingKey = _configuration["JwtConf:SigningKey"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!));
            var signCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole.Name),
                new Claim(ClaimTypes.GivenName, user.UserDetails.Firstname),
                new Claim(ClaimTypes.Surname, user.UserDetails.Lastname),
                new Claim("JobTitle", user.UserDetails.JobTitle)
            };

            var token = new JwtSecurityToken(issuer, audience, claims, null, DateTime.Now.AddHours(1), signCred);
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenStr;
        }
    }
}
