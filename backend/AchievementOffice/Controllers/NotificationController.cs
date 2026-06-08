using System.Security.Claims;
using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(
        INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationResponse>>>
        GetMyNotifications()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = 
            await _notificationService
                .GetUserNotificationsAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.ErrorMessage
            });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{notificationId:guid}")]
    public async Task<ActionResult>
        Delete(Guid notificationId)
    {
        var result =
            await _notificationService
                .DeleteAsync(notificationId);

        if (!result.IsSuccess)
        {
            return NotFound(new
            {
                message = result.ErrorMessage
            });
        }

        return NoContent();
    }
}