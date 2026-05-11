using AchievementOffice.Features.Achievements.DTOs;

namespace AchievementOffice.Features.Achievements;

public interface IAchievementService
{
    Task<AchievementResponseDto> CreateAchievementAsync(CreateAchievementDto createDto);

    Task<List<AchievementResponseDto>> GetAllAchievementsAsync();
}