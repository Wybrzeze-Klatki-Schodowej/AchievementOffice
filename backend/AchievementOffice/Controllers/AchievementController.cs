using AchievementOffice.Models;
using AchievementOffice.Services;
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

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AchievementResponse>> Update(Guid id, UpdateAchievementRequest dto)
    {
        var achievement = await _achievementService.UpdateAsync(id, dto);

        if (!achievement.IsSuccess)
            return NotFound(new { message = achievement.ErrorMessage });

        return Ok(achievement.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _achievementService.DeleteAsync(id);

        if (!deleted.IsSuccess)
            return NotFound(new { message = deleted.ErrorMessage });

        return NoContent();
    }

    [HttpPost( "{id:guid}/approve" )]
    public async Task<ActionResult<AchievementApproveResponseDto>> Approve(Guid id, CreateAchievementApproveDto dto)
    {
        try
        {
            var userId = Guid.Parse("11111111-2222-3333-4444-555555555555"); // pozniej z jwt, na razie hardcode
            var approve = await _achievementService.ApproveAsync(id, userId, dto);
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