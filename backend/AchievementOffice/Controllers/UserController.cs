using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
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
        public async Task<IActionResult> UpdateMe(
            UpdateUserRequest request)
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier
            );

            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(
                userIdClaim.Value
            );

            try
            {
                var updatedUser = 
                    await _userService.UpdateUserAsync(
                        userId,
                        request
                    );

                if (updatedUser == null)
                    return NotFound();

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }
    }
}