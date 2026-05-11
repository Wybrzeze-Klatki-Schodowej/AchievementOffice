using AchievementOffice.Data;
using AchievementOffice.Features.Achievements.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Features.Achievements;

public class AchievementService : IAchievementService
{
    private readonly AppDbContext _context;

    public AchievementService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AchievementResponseDto> CreateAchievementAsync(CreateAchievementDto dto)
    {
        var achievement = new Achievement
        {
            AchievementId = Guid.NewGuid(),
            UserId = dto.UserId,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Achievements.Add(achievement);
        await _context.SaveChangesAsync();

        return new AchievementResponseDto
        {
            AchievementId = achievement.AchievementId,
            UserId = achievement.UserId,
            Title = achievement.Title,
            Description = achievement.Description,
            CreatedAt = achievement.CreatedAt
        };
    }

    public async Task<List<AchievementResponseDto>> GetAllAchievementsAsync()
    {
        return await _context.Achievements
            .Where(a => a.DeletedAt == null)
            .Select(a => new AchievementResponseDto
            {
                AchievementId = a.AchievementId,
                UserId = a.UserId,
                Title = a.Title,
                Description = a.Description,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }
}