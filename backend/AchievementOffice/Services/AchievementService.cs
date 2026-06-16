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
    private readonly INotificationService _notificationService;

    public AchievementService(
        AppDbContext context, 
        IHttpContextAccessor httpContextAccessor,
        INotificationService notificationService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _notificationService = notificationService;
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
        achievement.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await ResetApprovalsAndRecreateRequestsAsync(id);

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

    public async Task<AchievementApproveResponseDto?> ApproveAsync(
        Guid achievementId, 
        Guid userId, 
        CreateAchievementApproveDto dto)
    {
        var existing = await _context.AchievementApproves
            .FirstOrDefaultAsync(a => 
                a.AchievementId == achievementId && 
                a.UserId == userId);

        var achievement = await _context.Achievements.FirstOrDefaultAsync(a => a.AchievementId == achievementId);

        if (achievement is null) return null;
        var ownerId = achievement.UserId;

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

            await RemoveVerificationNotificationAsync(achievementId, userId);
            await _context.SaveChangesAsync();

            return MapApproveToDto(approve, ownerId);
        }
            
        if (existing.DeletedAt != null)
        {
            existing.DeletedAt = null;
            existing.IsApproved = dto.IsApproved;
            existing.ApprovedAt = DateTime.UtcNow;

            await RemoveVerificationNotificationAsync(achievementId, userId);
            await _context.SaveChangesAsync();
            return MapApproveToDto(existing, ownerId);
        }
        
        if (existing.IsApproved == dto.IsApproved)
        {
            existing.DeletedAt = DateTime.UtcNow;
            await RemoveVerificationNotificationAsync(achievementId, userId);
            await _context.SaveChangesAsync();

            return new AchievementApproveResponseDto
            {
                AchievementApproveId = existing.AchievementApproveId,
                AchievementId = existing.AchievementId,
                UserId = existing.UserId,
                OwnerId = achievement.UserId,
                IsApproved = null,
                ApprovedAt = existing.ApprovedAt
            };
        }

        existing.IsApproved = dto.IsApproved;
        existing.ApprovedAt = DateTime.UtcNow;

        await RemoveVerificationNotificationAsync(achievementId, userId);
        await _context.SaveChangesAsync();

        return MapApproveToDto(existing, ownerId);
    }

    public async Task<List<AchievementApproveResponseDto>> GetApprovalsAsync(Guid achievementId)
    {
        var approvals = await _context.AchievementApproves
            .Where(a => a.AchievementId == achievementId && a.DeletedAt == null )
            .Include(a => a.User)
                .ThenInclude(u => u.UserDetails)
            .ToListAsync();

        var achievement = await _context.Achievements.FirstOrDefaultAsync(a => a.AchievementId == achievementId);

        return approvals.Select( a => MapApproveToDto(a, achievement?.UserId ?? Guid.Empty) ).ToList();
    }
    public async Task<AchievementApprovalsGroupedDto> GetApprovalsGroupedAsync(Guid achievementId)
    {
        var approvals = await _context.AchievementApproves
            .Where(a => a.AchievementId == achievementId && a.DeletedAt == null)
            .Include(a => a.User)
                .ThenInclude(u => u.UserDetails)
            .ToListAsync();

        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == achievementId);

        var ownerId = achievement?.UserId ?? Guid.Empty;

        return new AchievementApprovalsGroupedDto
        {
            Approved = approvals
                .Where(a => a.IsApproved == true)
                .Select(a => MapApproveToDto(a, ownerId))
                .ToList(),

            Denied = approvals
                .Where(a => a.IsApproved == false)
                .Select(a => MapApproveToDto(a, ownerId))
                .ToList()
        };
    }

    private static AchievementApproveResponseDto MapApproveToDto(AchievementApprove approve, Guid ownerId)
    {
        return new AchievementApproveResponseDto
        {
            AchievementApproveId = approve.AchievementApproveId,
            AchievementId = approve.AchievementId,
            UserId = approve.UserId,
            OwnerId = ownerId,
            UserLogin = approve.User?.Login,
            UserFirstName = approve.User?.UserDetails?.Firstname,
            UserLastName = approve.User?.UserDetails?.Lastname,
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
            .Where(a => a.UserId == userId && a.DeletedAt == null)
            .ToListAsync();

        return achievements.Select(MapToDto).ToList();
    }

    private async Task ResetApprovalsAndRecreateRequestsAsync(Guid achievementId)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == achievementId && a.DeletedAt == null);

        if (achievement == null)
            return;

        var previousVoters = await _context.AchievementApproves
            .Where(a => a.AchievementId == achievementId)
            .Select(a => a.UserId)
            .Distinct()
            .ToListAsync();

        _context.AchievementApproves.RemoveRange(
            _context.AchievementApproves.Where(a => a.AchievementId == achievementId)
        );

        var pendingRequests = await _context.AchievementVerificationRequests
            .Where(r => r.AchievementId == achievementId &&
                r.Status == VerificationRequestStatus.Pending)
            .ToListAsync();

        _context.AchievementVerificationRequests.RemoveRange(pendingRequests);

        await _context.SaveChangesAsync();

        var requesterId = achievement.UserId;

        foreach (var userId in previousVoters.Distinct())
        {
            if (userId == requesterId)
                continue;

            var request = new AchievementVerificationRequest
            {
                Id = Guid.NewGuid(),
                AchievementId = achievementId,
                RequesterUserId = requesterId,
                TargetUserId = userId,
                Status = VerificationRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.AchievementVerificationRequests.Add(request);
            await _context.SaveChangesAsync();

            await _notificationService.CreateAchievementVerificationRequestNotificationAsync(
                userId,
                request.Id,
                achievement.Title
            );
        }
    }

    private async Task RemoveVerificationNotificationAsync(Guid achievementId, Guid userId)
    {
        var request = await _context.AchievementVerificationRequests
            .FirstOrDefaultAsync(r =>
                r.AchievementId == achievementId &&
                r.TargetUserId == userId);

        if (request == null) 
            return;

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n =>
                n.AchievementVerificationRequestId == request.Id);

        if (notification != null)
        {
            _context.Notifications.Remove(notification);
        }
    }
}