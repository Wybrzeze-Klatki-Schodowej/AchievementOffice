using System.Security.Claims;
using AchievementOffice.Common;
using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Controllers;

[ApiController]
[Route("api/achievements")]
public class AchievementController : ControllerBase
{
    private readonly IAchievementService _achievementService;

    public AchievementController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AchievementResponse>>> GetAll()
    {
        var achievements = await _achievementService.GetAllAsync();

        if (!achievements.IsSuccess)
            return BadRequest(new {message = achievements.ErrorMessage});

        return Ok(achievements.Value);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AchievementResponse>> Create(CreateAchievementRequest dto)
    {
        var achievement = await _achievementService.CreateAsync(dto);

        if (!achievement.IsSuccess)
            return BadRequest(new { message = achievement.ErrorMessage });

        return CreatedAtAction(
            nameof(GetById),
            new { id = achievement.Value!.AchievementId },
            achievement.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AchievementResponse>> GetById(Guid id)
    {
        var achievement = await _achievementService.GetByIdAsync(id);

        if (!achievement.IsSuccess)
            return NotFound(new { message = achievement.ErrorMessage });

        return Ok(achievement.Value);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AchievementResponse>> Update(Guid id, UpdateAchievementRequest dto)
    {
        var updated = await _achievementService.UpdateAsync(id, dto);

        if (!updated.IsSuccess)
            return updated.ErrorMessage switch
            {
                "Not found" => NotFound(),
                "Forbidden" => Forbid(),
                _ => BadRequest(new { message = updated.ErrorMessage })
            };

        return Ok(updated.Value);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {       
        var deleted = await _achievementService.DeleteAsync(id);

        if (!deleted.IsSuccess)
            return deleted.ErrorMessage switch
            {
                "Not found" => NotFound(),
                "Forbidden" => Forbid(),
                _ => BadRequest(new { message = deleted.ErrorMessage })
            };

        return NoContent();
    }

    [HttpPost( "{id:guid}/approve" )]
    public async Task<ActionResult<AchievementApproveResponseDto>> Approve(Guid id, CreateAchievementApproveDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user ID in token." });
            
            var approve = await _achievementService.ApproveAsync(userId, dto);
            return Ok(approve);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpGet( "{id:guid}/approvals" )]
    public async Task<ActionResult<List<AchievementApproveResponseDto>>> GetApprovals(Guid id)
    {
        var approvals = await _achievementService.GetApprovalsAsync( id );
        return Ok( approvals );
    }

    [HttpGet("{id:guid}/approvals/summary" )]
    public async Task<ActionResult<AchievementApprovalSummaryDto>> GetApprovalSummary(Guid id)
    {
        var summary = await _achievementService.GetApprovalSummaryAsync( id );
        return Ok( summary );
    }
}