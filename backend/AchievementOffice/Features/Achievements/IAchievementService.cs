using AchievementOffice.Features.Achievements.DTOs;

namespace AchievementOffice.Features.Achievements;

public interface IAchievementService
{
    Task<AchievementResponseDto> CreateAsync(CreateAchievementDto createDto);

    Task<List<AchievementResponseDto>> GetAllAsync();

    Task<AchievementResponseDto?> GetByIdAsync(Guid id);

    Task<AchievementResponseDto?> UpdateAsync(Guid id, UpdateAchievementDto dto);

    Task<bool> DeleteAsync(Guid id);
}