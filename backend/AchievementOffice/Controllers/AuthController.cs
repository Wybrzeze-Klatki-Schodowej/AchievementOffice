using AchievementOffice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AchievementOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (loginModel is null) return BadRequest("Invalid request");

            //TODO: Add to Claim list elements of User Entity
            if (loginModel.UserName == "Test" && loginModel.Password == "1234")
            {
                var issuer = _configuration["JwtConf:Issuer"];
                var audience = _configuration["JwtConf:Audience"];
                var signingKey = _configuration["JwtConf:SigningKey"];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!));
                var signCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var opts = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signCred
                );

                var tokenStr = new JwtSecurityTokenHandler().WriteToken(opts);

                var coockieOpts = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddHours(1)
                };

                Response.Cookies.Append("X-jwt-token", tokenStr, coockieOpts);
                return Ok();
            }
            return Unauthorized("Invalid login or password");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("X-jwt-token");
            return Ok();
        }

        [Authorize]
        [HttpGet("mock-data")]
        public IActionResult MockGet()
        {
            var username = User.Identity?.Name ?? "Nieznajomy";
            return Ok(new { message = $"Wszedłeś! Twój token działa. Cześć, {username}!" });
        }
    }
}