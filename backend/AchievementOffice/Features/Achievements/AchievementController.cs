using AchievementOffice.Features.Achievements.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Features.Achievements;

[ApiController]
[Route("api/[controller]")]
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
        var achievements = await _achievementService.GetAllAchievementsAsync();
        return Ok(achievements);
    }

    [HttpPost]
    public async Task<ActionResult<AchievementResponseDto>> Create(CreateAchievementDto dto)
    {
        var achievement = await _achievementService.CreateAchievementAsync(dto);
        return Ok(achievement);
    }
}