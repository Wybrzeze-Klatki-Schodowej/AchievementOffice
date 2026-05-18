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

    public async Task<AchievementApproveResponseDto> ApproveAsync(Guid userId, CreateAchievementApproveDto dto)
    {
        var existing = await _context.AchievementApproves
        .FirstOrDefaultAsync(a => a.AchievementId == dto.AchievementId
                                && a.UserId == userId
                                && a.DeletedAt == null);

        if (existing != null)
            throw new InvalidOperationException("User already voted on this achievement");

        var approve = new AchievementApprove
        {
            AchievementApproveId = Guid.NewGuid(),
            AchievementId = dto.AchievementId,
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