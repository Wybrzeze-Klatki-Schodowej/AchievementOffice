using System.Runtime.CompilerServices;
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

    public async Task<AchievementResponseDto> CreateAsync(CreateAchievementDto dto)
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

        return MapToDto(achievement);
    }

    public async Task<List<AchievementResponseDto>> GetAllAsync()
    {
        var achievements = await _context.Achievements
            .Where(a => a.DeletedAt == null)
            .ToListAsync();
        return achievements.Select(MapToDto).ToList();
    }

    public async Task<AchievementResponseDto?> GetByIdAsync(Guid id)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == id && a.DeletedAt == null);

        if (achievement == null)
            return null;

        return MapToDto(achievement);
    }

    public async Task<AchievementResponseDto?> UpdateAsync(Guid id, UpdateAchievementDto dto)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == id && a.DeletedAt == null);

        if (achievement == null)
            return null;

        achievement.Title = dto.Title;
        achievement.Description = dto.Description;
        achievement.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(achievement);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == id && a.DeletedAt == null);

        if (achievement == null)
            return false;

        achievement.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    private static AchievementResponseDto MapToDto(Achievement achievement)
    {
        return new AchievementResponseDto
        {
            AchievementId = achievement.AchievementId,
            UserId = achievement.UserId,
            Title = achievement.Title,
            Description = achievement.Description,
            CreatedAt = achievement.CreatedAt,
            UpdatedAt = achievement.UpdatedAt
        };
    }
}