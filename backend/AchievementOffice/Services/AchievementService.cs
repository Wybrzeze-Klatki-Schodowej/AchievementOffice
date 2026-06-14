using AchievementOffice.Data;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AchievementOffice.Entities;

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
        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result<AchievementResponse>.Fail("Title is required.");

        var achievement = new Achievement
        {
            AchievementId = Guid.NewGuid(),
            UserId = GetUserId(),
            Title = dto.Title,
            Description = dto.Description,
            VisibilityId = dto.VisibilityId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Achievements.Add(achievement);

        if (dto.VisibilityId == (int)VisibilityMode.Group)
        {
            if (dto.GroupIds == null || !dto.GroupIds.Any())
                return Result<AchievementResponse>.Fail("At least one group is required when visibility is set to Groups");

            var groups = dto.GroupIds.Distinct().Select(gId => new AchievementGroup
            {
                AchievementId = achievement.AchievementId,
                GroupId = gId
            });
            _context.AchievementGroups.AddRange(groups);
        }
        else if (dto.GroupIds?.Any() == true)
        {
            return Result<AchievementResponse>.Fail("GroupIds can only be provided when visibility is set to Groups");
        }

        await _context.SaveChangesAsync();

        return Result<AchievementResponse>.Success(MapToDto(achievement));
    }

    public async Task<Result<List<AchievementResponse>>> GetAllAsync()
    {
        var userId = GetUserId();
        var role = GetRole();
        var isAdmin = role == "Admin";

        var query = _context.Achievements
            .AsQueryable()
            .Where(a => a.DeletedAt == null);

        if (!isAdmin)
        {
            query = query.Where(a =>
                a.UserId == userId ||
                a.VisibilityId == (int)VisibilityMode.Public ||
                (a.VisibilityId == (int)VisibilityMode.Group &&
                    a.AchievementGroups.Any(g =>
                        _context.GroupUsers.Any(ug =>
                            ug.UserId == userId && ug.GroupId == g.GroupId
                        )
                    )
                )
            );
        }

        var achievements = await query
            .Include(a => a.AchievementGroups)
            .ToListAsync();

        var result = achievements.Select(MapToDto).ToList();
        return Result<List<AchievementResponse>>.Success(result);
    }

    public async Task<Result<AchievementResponse>> GetByIdAsync(Guid id)
    {
        var achievement = await _context.Achievements
            .Include(a => a.AchievementGroups)
            .FirstOrDefaultAsync(a => a.AchievementId == id && a.DeletedAt == null);

        if (achievement == null)
            return Result<AchievementResponse>.Fail("Achievement not found.");

        return Result<AchievementResponse>.Success(MapToDto(achievement));
    }

    public async Task<Result<AchievementResponse>> UpdateAsync(Guid id, UpdateAchievementRequest dto)
    {
        var achievement = await _context.Achievements
            .Include(a => a.AchievementGroups)
            .FirstOrDefaultAsync(a => a.AchievementId == id && a.DeletedAt == null);

        if (achievement == null)
            return Result<AchievementResponse>.Fail("Achievement not found.");

        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result<AchievementResponse>.Fail("Title is required.");

        var userId = GetUserId();
        var role = GetRole();

        var isOwner = achievement.UserId == userId;
        var isAdmin = role == "Admin";

        if (!isOwner && !isAdmin)
            return Result<AchievementResponse>.Fail("Forbidden");

        achievement.Title = dto.Title;
        achievement.Description = dto.Description;
        achievement.VisibilityId = dto.VisibilityId;
        achievement.UpdatedAt = DateTime.UtcNow;

        _context.AchievementGroups.RemoveRange(achievement.AchievementGroups);

        if (dto.VisibilityId == (int)VisibilityMode.Group)
        {
            if (dto.GroupIds == null || !dto.GroupIds.Any())
                return Result<AchievementResponse>.Fail("At least one group is required when visibility is set to Groups");

            var groups = dto.GroupIds.Distinct().Select(gId => new AchievementGroup
            {
                AchievementId = achievement.AchievementId,
                GroupId = gId
            });
            _context.AchievementGroups.AddRange(groups);
        }
        else if (dto.GroupIds?.Any() == true)
        {
            return Result<AchievementResponse>.Fail("GroupIds can only be provided when visibility is set to Groups");
        }

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
            VisibilityId = achievement.VisibilityId,
            GroupIds = achievement.AchievementGroups
                .Select(g => g.GroupId)
                .ToList(),
            CreatedAt = achievement.CreatedAt,
            UpdatedAt = achievement.UpdatedAt
        };
    }

    public async Task<AchievementApproveResponseDto> ApproveAsync(
        Guid achievementId, 
        Guid userId, 
        CreateAchievementApproveDto dto)
    {
        var existing = await _context.AchievementApproves
            .FirstOrDefaultAsync(a => 
                a.AchievementId == achievementId && 
                a.UserId == userId);

        if (existing is null)
        {
            var approve = new AchievementApprove
            {
                AchievementApproveId = Guid.NewGuid(),
                AchievementId = achievementId,
                UserId = userId,
                IsApproved = dto.IsApproved,
                ApprovedAt = DateTime.UtcNow
            };

            _context.AchievementApproves.Add(approve);
            await _context.SaveChangesAsync();

            return MapApproveToDto(approve);
        }
            
        if (existing.DeletedAt != null)
        {
            existing.DeletedAt = null;
            existing.IsApproved = dto.IsApproved;
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

        existing.IsApproved = dto.IsApproved;
        existing.ApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapApproveToDto(existing);
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
            .Where( a => 
                a.AchievementId == achievementId && 
                a.DeletedAt == null )
            .ToListAsync();

        var userId = GetUserId();

        var myVote = approvals
            .FirstOrDefault(a => a.UserId == userId)
            ?.IsApproved;

        return new AchievementApprovalSummaryDto
        {
            Approved = approvals.Count( a => a.IsApproved ),
            Denied = approvals.Count( a => !a.IsApproved ),
            CurrentUserVote = myVote
        };
    }

    public async Task<List<AchievementResponse>> GetByUserIdAsync(Guid userId)
    {
        var achievements = await _context.Achievements
            .Include(a => a.AchievementGroups)
            .Where(a => a.UserId == userId && a.DeletedAt == null)
            .ToListAsync();

        return achievements.Select(MapToDto).ToList();
    }
}