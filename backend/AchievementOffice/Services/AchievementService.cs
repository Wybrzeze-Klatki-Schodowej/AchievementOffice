using AchievementOffice.Data;
using AchievementOffice.Common;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Services;

public class AchievementService : IAchievementService
{
    private readonly AppDbContext _context;

    public AchievementService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AchievementResponse>> CreateAsync(CreateAchievementRequest dto)
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

        return Result<AchievementResponse>.Success(MapToDto(achievement));
    }

    public async Task<Result<List<AchievementResponse>>> GetAllAsync()
    {
        var achievements = await _context.Achievements
            .Where(a => a.DeletedAt == null)
            .ToListAsync();

        var result = achievements.Select(MapToDto).ToList();

        return Result<List<AchievementResponse>>.Success(result);
    }

    public async Task<Result<AchievementResponse>> GetByIdAsync(Guid id)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == id && a.DeletedAt == null);

        if (achievement == null)
            return Result<AchievementResponse>.Fail("Achievement not found.");

        return Result<AchievementResponse>.Success(MapToDto(achievement));
    }

    public async Task<Result<AchievementResponse>> UpdateAsync(Guid id, UpdateAchievementRequest dto)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == id && a.DeletedAt == null);

        if (achievement == null)
            return Result<AchievementResponse>.Fail("Achievement not found.");

        achievement.Title = dto.Title;
        achievement.Description = dto.Description;
        achievement.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<AchievementResponse>.Success(MapToDto(achievement));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == id && a.DeletedAt == null);

        if (achievement == null)
            return Result<bool>.Fail("Achievement not found.");

        achievement.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    private static AchievementResponse MapToDto(Achievement achievement)
    {
        return new AchievementResponse
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