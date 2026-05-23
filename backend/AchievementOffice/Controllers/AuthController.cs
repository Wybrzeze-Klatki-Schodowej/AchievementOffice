using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AchievementOffice.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _userService.LoginAsync(request);

        if (!result.IsSuccessful)
            return Unauthorized(new { Message = "Invalid username or password" });

        var opts = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.Now.AddHours(1)
        };
        Response.Cookies.Append("X-jwt-token", result.Token! , opts);

        return Ok();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var opts = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
        };
        Response.Cookies.Delete("X-jwt-token", opts);
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

    [HttpGet("is-logged")]
    public ActionResult<bool> IsLogged()
    {
        if (User.Identity?.IsAuthenticated != true) return Ok(false);

        return Ok(true);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var login = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            userId,
            login,
            email,
            role
        });
    }
}