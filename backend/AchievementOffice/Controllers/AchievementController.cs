using AchievementOffice.Features.Achievements.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Features.Achievements;

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
    public async Task<ActionResult<List<AchievementResponseDto>>> GetAll()
    {
        var achievements = await _achievementService.GetAllAsync();
        return Ok(achievements);
    }

    [HttpPost]
    public async Task<ActionResult<AchievementResponseDto>> Create(CreateAchievementDto dto)
    {
        var achievement = await _achievementService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = achievement.AchievementId },
            achievement);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AchievementResponseDto>> GetById(Guid id)
    {
        var achievement = await _achievementService.GetByIdAsync(id);

        if (achievement == null)
            return NotFound();

        return Ok(achievement);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AchievementResponseDto>> Update(Guid id, UpdateAchievementDto dto)
    {
        var achievement = await _achievementService.UpdateAsync(id, dto);

        if (achievement == null)
            return NotFound();

        return Ok(achievement);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _achievementService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost( "{id:guid}/approve" )]
    public async Task<ActionResult<AchievementApproveResponseDto>> Approve(Guid id, CreateAchievementApproveDto dto)
    {
        try
        {
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000101"); // pozniej z jwt, na razie hardcode
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