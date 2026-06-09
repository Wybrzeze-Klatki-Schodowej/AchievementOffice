using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Services;

public class AchievementVerificationRequestService
    : IAchievementVerificationRequestService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public AchievementVerificationRequestService(
        AppDbContext context,
        INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Result<AchievementVerificationRequestResponse>>
        CreateAsync(
            Guid achievementId,
            Guid requesterUserId,
            CreateAchievementVerificationRequestDto request)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a =>
                a.AchievementId == achievementId &&
                a.DeletedAt == null);

        if (achievement == null)
        {
            return Result<AchievementVerificationRequestResponse>
                .Fail("Achievement not found.");
        }

        var targetUser = await _context.Users
            .FirstOrDefaultAsync(u =>
            u.Id == request.TargetUserId &&
            u.DeletedAt == null);

        if (targetUser == null)
        {
            return Result<AchievementVerificationRequestResponse>
                .Fail("Target user not found");
        }

        if (achievement.UserId == request.TargetUserId)
        {
            return Result<AchievementVerificationRequestResponse>
                .Fail("You cannot request verification from yourself");
        }

        var alreadyVoted = await _context.AchievementApproves
            .AnyAsync(a =>
                a.AchievementId == achievementId &&
                a.UserId == request.TargetUserId &&
                a.DeletedAt == null);

        if (alreadyVoted)
        {
            return Result<AchievementVerificationRequestResponse>
                .Fail("User has already voted.");
        }

        var pendingRequestExists =
            await _context.AchievementVerificationRequests
                .AnyAsync(r =>
                    r.AchievementId == achievementId &&
                    r.TargetUserId == request.TargetUserId &&
                    r.Status == VerificationRequestStatus.Pending);

        if (pendingRequestExists)
        {
            return Result<AchievementVerificationRequestResponse>
                .Fail("Verification request already exists.");
        }

        var verificationRequest =
            new AchievementVerificationRequest
            {
                Id = Guid.NewGuid(),
                AchievementId = achievementId,
                RequesterUserId = requesterUserId,
                TargetUserId = request.TargetUserId,
                Status = VerificationRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

        _context.AchievementVerificationRequests
            .Add(verificationRequest);

        await _context.SaveChangesAsync();

        var notificationResult =
            await _notificationService
                .CreateAchievementVerificationRequestNotificationAsync(
                    request.TargetUserId,
                    verificationRequest.Id,
                    achievement.Title);

        if (!notificationResult.IsSuccess)
        {
            return Result<AchievementVerificationRequestResponse>
                .Fail(notificationResult.ErrorMessage!);
        }

        return Result<AchievementVerificationRequestResponse>
            .Success(MapToResponse(
                verificationRequest,
                string.Empty,
                targetUser.Login,
                achievement.Title));
    }

    public async Task<Result>
        ApproveAsync(
            Guid requestId,
            Guid userId)
    {
        return await HandleDecisionAsync(
            requestId,
            userId,
            true,
            VerificationRequestStatus.Approved);
    }

    public async Task<Result>
        RejectAsync(
            Guid requestId,
            Guid userId)
    {
        return await HandleDecisionAsync(
            requestId,
            userId,
            false,
            VerificationRequestStatus.Denied);
    }

    public async Task<Result<List<AchievementVerificationRequestResponse>>>
        GetPendingForUserAsync(Guid userId)
    {
        var requests = 
            await _context.AchievementVerificationRequests
                .Include(r => r.Achievement)
                .Include(r => r.RequesterUser)
                .Include(r => r.TargetUser)
                .Where(r =>
                    r.TargetUserId == userId &&
                    r.Status == VerificationRequestStatus.Pending)
                .ToListAsync();

        var response = requests
            .Select(r =>
                MapToResponse(
                    r,
                    r.RequesterUser.Login,
                    r.TargetUser.Login,
                    r.Achievement.Title))
            .ToList();

        return Result<List<AchievementVerificationRequestResponse>>
            .Success(response);
    }

    private async Task<Result>
        HandleDecisionAsync(
            Guid requestId,
            Guid userId,
            bool isApproved,
            VerificationRequestStatus status)
    {
        var request = 
            await _context.AchievementVerificationRequests
                .FirstOrDefaultAsync(r =>
                    r.Id == requestId);

        if (request == null)
        {
            return Result.Fail("Verification request not found");
        }

        if (request.TargetUserId != userId)
        {
            return Result.Fail("Forbidden.");
        }

        var existingVote = 
            await _context.AchievementApproves
                .FirstOrDefaultAsync(a =>
                    a.AchievementId == request.AchievementId &&
                    a.UserId == userId);

        if (existingVote == null)
        {
            _context.AchievementApproves.Add(
                new AchievementApprove
                {
                    AchievementApproveId = Guid.NewGuid(),
                    AchievementId = request.AchievementId,
                    UserId = userId,
                    IsApproved = isApproved,
                    ApprovedAt = DateTime.UtcNow
                });
        }
        else
        {
            existingVote.IsApproved = isApproved;
            existingVote.ApprovedAt = DateTime.UtcNow;
            existingVote.DeletedAt = null;
        }

        request.Status = status;
        request.RespondedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var notification =
            await _context.Notifications
                .FirstOrDefaultAsync(n =>
                n.AchievementVerificationRequestId ==
                request.Id);

        if (notification != null)
        {
            var deleteResult =
                await _notificationService
                    .DeleteAsync(notification.Id);

            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }
        }

        return Result.Success();
    }

    private static AchievementVerificationRequestResponse
        MapToResponse(
            AchievementVerificationRequest request,
            string requesterLogin,
            string targetLogin,
            string achievementTitle)
    {
        return new AchievementVerificationRequestResponse
        {
            Id = request.Id,
            AchievementId = request.AchievementId,
            AchievementTitle = achievementTitle,
            RequesterUserId = request.RequesterUserId,
            RequesterLogin = requesterLogin,
            TargetUserId = request.TargetUserId,
            TargetUserLogin = targetLogin,
            Status = request.Status.ToString(),
            CreatedAt = request.CreatedAt,
            RespondedAt = request.RespondedAt
        };
    }

    public async Task<Result<List<UserReviewerResponse>>>
        GetAvailableReviewersAsync(Guid achievementId)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a =>
                a.AchievementId == achievementId &&
                a.DeletedAt == null);

        if (achievement == null)
        {
            return Result<List<UserReviewerResponse>>
                .Fail("Achievement not found");
        }

        var users = await _context.Users
            .Include(u => u.UserDetails)
            .Where(u =>
                u.DeletedAt == null &&
                u.IsActive)
            .ToListAsync();

        var availableUsers = users
            .Where(u =>
                u.Id != achievement.UserId)
            .Where(u =>
                !_context.AchievementApproves.Any(a =>
                    a.AchievementId == achievementId &&
                    a.UserId == u.Id &&
                    a.DeletedAt == null))
            .Where(u =>
                !_context.AchievementVerificationRequests.Any(r =>
                    r.AchievementId == achievementId &&
                    r.TargetUserId == u.Id &&
                    r.Status == VerificationRequestStatus.Pending))
            .Select(u => new UserReviewerResponse
            {
                UserId = u.Id,
                Login = u.Login,
                FirstName = u.UserDetails.Firstname,
                LastName = u.UserDetails.Lastname
            })
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToList();

        return Result<List<UserReviewerResponse>>
            .Success(availableUsers);
    }

    public async Task<Result<AchievementVerificationRequestResponse>>
        GetByIdAsync(Guid requestId)
    {
        var request =
            await _context.AchievementVerificationRequests
                .Include(r => r.Achievement)
                .Include(r => r.RequesterUser)
                .Include(r => r.TargetUser)
                .FirstOrDefaultAsync(r =>
                    r.Id == requestId);

        if (request == null)
        {
            return Result<AchievementVerificationRequestResponse>
                .Fail("Verification request not found");
        }

        return Result<AchievementVerificationRequestResponse>
            .Success(
                MapToResponse(
                    request,
                    request.RequesterUser.Login,
                    request.TargetUser.Login,
                    request.Achievement.Title));
    }
}