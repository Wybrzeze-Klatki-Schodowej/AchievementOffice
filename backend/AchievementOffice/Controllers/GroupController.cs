using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("User ID could not be found in the token.");
            }

            var result = await _groupService.CreateGroupAsync(request, userId);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.ErrorMessage });

            return Created(string.Empty, new { groupId =  result.Value});
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetGroups()
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("User ID could not be found in the token.");
            }

            var result = await _groupService.GetGroupsAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.ErrorMessage });

            return Ok(result.Value);
        }

        [HttpPost("{groupId}/members")]
        [Authorize]
        public async Task<IActionResult> AddUserToGroup([FromRoute] Guid groupId, [FromBody] AddUserToGroupRequest request)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("User ID could not be found in the token.");
            }

            var result = await _groupService.AddUserToGroupAsync(groupId, request.UserId!.Value, request.RoleId!.Value, userId);

            if(!result.IsSuccess)
            {
                if (result.ErrorMessage?.Contains("permission") == true)
                    return Forbid(result.ErrorMessage);

                return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }

        [HttpGet("{groupId}")]
        [Authorize]
        public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId)
        {
            var result = await _groupService.GetGroupByIdAsync(groupId);

            if (!result.IsSuccess)
            {
                return NotFound(new { error = result.ErrorMessage });
            }

            return Ok(result.Value);
        }

        [HttpGet("{groupId}/members")]
        [Authorize]
        public async Task<IActionResult> GetGroupMembers([FromRoute] Guid groupId)
        {
            var result = await _groupService.GetGroupMembersAsync(groupId);

            if (!result.IsSuccess)
            {
                return NotFound(new { error = result.ErrorMessage });
            }

            return Ok(result.Value);
        }

        [HttpGet("{groupId}/roles")]
        [Authorize]
        public async Task<IActionResult> GetGroupRoles([FromRoute] Guid groupId)
        {
            var result = await _groupService.GetGroupRolesAsync(groupId);

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{groupId}/members/{userId}")]
        [Authorize]
        public async Task<IActionResult> RemoveUserFromGroup(
            [FromRoute] Guid groupId,
            [FromRoute] Guid userId)
        {
            var currentUserIdString = 
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(currentUserIdString, out Guid currentUserId))
            {
                return Unauthorized();
            }

            var isGlobalAdmin = User.IsInRole("Admin");

            var result = await _groupService.RemoveUserFromGroupAsync(
                groupId,
                userId,
                currentUserId,
                isGlobalAdmin);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage == "Forbidden")
                    return Forbid();

                return BadRequest(new
                {
                    error = result.ErrorMessage
                });
            }

            return NoContent();
        }
    }
}
