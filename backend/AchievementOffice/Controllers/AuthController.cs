using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _userService.LoginAsync(request);

            if (!result.IsSuccesful)
                return Unauthorized(new { Message = "Invalid username or password" });

            var opts = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddHours(1)
            };
            Response.Cookies.Append("X-jwt-token", result.Token! , opts);

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            var result = await _userService.RegisterUserAsync(request);

            if (!result.IsSuccessful)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok();
        }

        [HttpGet("me")]
        public async Task<Boolean> IsLogged()
        {
            return User.Identity?.IsAuthenticated ?? false;
        }

    }
}
