using AchievementOffice.Data;
using AchievementOffice.Common;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

    public async Task<AchievementApproveResponseDto> ApproveAsync(Guid achievementId, Guid userId, CreateAchievementApproveDto dto)
    {
        var existing = await _context.AchievementApproves
        .FirstOrDefaultAsync(a => a.AchievementId == achievementId
                                && a.UserId == userId);

        if (existing != null)
        {
            if (existing.DeletedAt != null)
            {
                existing.DeletedAt = null; // Reanimujemy wiersz
                existing.IsApproved = dto.IsApproved; // Ustawiamy stan przycisku na nowy
                existing.ApprovedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return MapApproveToDto(existing);
            }

            if (existing.IsApproved == dto.IsApproved)
            {
                existing.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new AchievementApproveResponseDto
                {
                    AchievementApproveId = existing.AchievementApproveId,
                    AchievementId = existing.AchievementId,
                    UserId = existing.UserId,
                    IsApproved = null,
                    ApprovedAt = existing.ApprovedAt
                };
            }
            else
            {
                existing.IsApproved = dto.IsApproved;
                existing.ApprovedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return MapApproveToDto(existing);
            }
        }

        var approve = new AchievementApprove
        {
            AchievementApproveId = Guid.NewGuid(),
            AchievementId = achievementId,
            UserId = userId,
            IsApproved = dto.IsApproved,
            ApprovedAt = DateTime.UtcNow
        };
        _context.AchievementApproves.Add( approve );
        await _context.SaveChangesAsync();
        return MapApproveToDto( approve );
    }

    public async Task<List<AchievementApproveResponseDto>> GetApprovalsAsync(Guid achievementId)
    {
        var approvals = await _context.AchievementApproves
            .Where( a => a.AchievementId == achievementId && a.DeletedAt == null )
            .ToListAsync();
        return approvals.Select( MapApproveToDto ).ToList();
    }

    private static AchievementApproveResponseDto MapApproveToDto(AchievementApprove approve)
    {
        return new AchievementApproveResponseDto
        {
            AchievementApproveId = approve.AchievementApproveId,
            AchievementId = approve.AchievementId,
            UserId = approve.UserId,
            IsApproved = approve.IsApproved,
            ApprovedAt = approve.ApprovedAt
        };
    }

    public async Task<AchievementApprovalSummaryDto> GetApprovalSummaryAsync(Guid achievementId)
    {
        var approvals = await _context.AchievementApproves
            .Where( a => a.AchievementId == achievementId && a.DeletedAt == null )
            .ToListAsync();
        return new AchievementApprovalSummaryDto
        {
            Approved = approvals.Count( a => a.IsApproved ),
            Denied = approvals.Count( a => !a.IsApproved )
        };
    }
}