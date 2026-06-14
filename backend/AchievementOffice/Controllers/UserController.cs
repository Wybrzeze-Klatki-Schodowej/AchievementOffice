using System.Security.Claims;
using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AchievementOffice.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAchievementService _achievementService;

        public UserController(
            IUserService userService,
            IAchievementService achievementService)
        {
            _userService = userService;
            _achievementService = achievementService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(
            Guid userId)
        {
            var user = await _userService.GetUserProfileAsync(userId);
        
        
        if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(user);
        }
        
        // [HttpGet]
        // public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        // {
        //     var users = await _userService.GetAllUsersAsync2();

        //     return Ok(users);
        // }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{userId:guid}/achievements")]
        public async Task<IActionResult> GetUserAchievements(Guid userId)
        {
            var achievements = await _achievementService.GetByUserIdAsync(userId);
            return Ok(achievements);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(UpdateUserRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            var result = 
                await _userService.UpdateUserAsync(
                    userId,
                    request
                );

            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    Message = result.ErrorMessage
                });
            }

            return Ok(result.Value);
        }

        [HttpPut("me/password")]
        public async Task<IActionResult>
            ChangePassword(ChangePasswordRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            var result = 
                await _userService
                    .ChangePasswordAsync(
                        userId,
                        request
                    );

            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    Message = result.ErrorMessage   
                });
            }

            return Ok(new
            {
                Message = "Password changed successfully"
            });
        }
    }
}

/*
public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync2();

            return Ok(users);
        }
    }
*/