using AchievementOffice.Data;
using AchievementOffice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AchievementOffice.Services;

namespace AchievementOffice.Controllers
{
    [ApiController]
    [Route( "api/admin" )]
    [Authorize( Roles = "Admin" )]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet( "users" )]
        public async Task<IActionResult> GetAllUsers([FromQuery] bool? isActive)
        {
            var users = await _adminService.GetAllUsersAsync(isActive);
            return Ok( users );
        }

        [HttpPatch( "users/{id:guid}/status" )]
        public async Task<IActionResult> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusRequest request)
        {
            var result = await _adminService.UpdateUserStatusAsync( id, request.IsActive );
            if (!result.IsSuccess)
            {
                return NotFound( new { message = result.ErrorMessage } );
            }
            return Ok( new { message = "User status updated successfully" } );
        }

        [HttpGet( "stats" )]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _adminService.GetStatsAsync();
            return Ok( stats );
        }

        [HttpGet("ranks")]
        public async Task<IActionResult> GetRanks()
        {
            var ranks = await _adminService.GetRanksAsync();
            return Ok( ranks );
        }

        [HttpPatch("users/{id:guid}/rank")]
        public async Task<IActionResult> UpdateUserRank(Guid id, [FromBody] UpdateUserRankRequest request)
        {
            var result = await _adminService.UpdateUserRankAsync(id, request.RankId);
            if (!result.IsSuccess)
            {
                return NotFound(new { message = result.ErrorMessage });
            }
            return Ok(new { message = "User rank updated successfully" });
        }

        [HttpDelete("comments/{id:guid}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var result = await _adminService.DeleteCommentAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });
            return Ok(new { message = "Comment deleted successfully" });
        }

        [HttpDelete("achievements/{id:guid}")]
        public async Task<IActionResult> DeleteAchievement(Guid id)
        {
            var result = await _adminService.DeleteAchievementAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });
            return Ok(new { message = "Achievement deleted successfully" });
        }

        [HttpPost("ranks")]
        public async Task<IActionResult> CreateRank([FromBody] CreateRankRequest request)
        {
            var result = await _adminService.CreateRankAsync(request);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });
            return Ok(new { message = "Rank created successfully" });
        }
    }
}