using AchievementOffice.Data;
using AchievementOffice.Common;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AchievementOffice.Services;

public class AchievementService : IAchievementService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AchievementService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<AchievementResponse>> CreateAsync(
        CreateAchievementRequest dto
    )
    {
        var achievement = new Achievement
        {
            AchievementId = Guid.NewGuid(),
            UserId = GetUserId(),
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

        var userId = GetUserId();
        var role = GetRole();

        var isOwner = achievement.UserId == userId;
        var isAdmin = role == "Admin";

        if (!isOwner && !isAdmin)
            return Result<AchievementResponse>.Fail("Forbidden");

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
        
        var userId = GetUserId();
        var role = GetRole();

        var isOwner = achievement.UserId == userId;
        var isAdmin = role == "Admin";

        if (!isOwner && !isAdmin)
            return Result<bool>.Fail("Forbidden");

        achievement.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    private Guid GetUserId()
    {
        var context = _httpContextAccessor.HttpContext;

        if (context == null)
            return Guid.Empty;

        var claim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    private string? GetRole()
    {
        return _httpContextAccessor.HttpContext?
            .User.FindFirstValue(ClaimTypes.Role);
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

    public async Task<List<AchievementResponse>> GetByUserIdAsync(Guid userId)
    {
        var achievements = await _context.Achievements
            .Where(a => a.UserId == userId && a.DeletedAt == null)
            .ToListAsync();

        return achievements.Select(MapToDto).ToList();
    }
}