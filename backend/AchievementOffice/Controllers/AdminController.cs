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
    }
}