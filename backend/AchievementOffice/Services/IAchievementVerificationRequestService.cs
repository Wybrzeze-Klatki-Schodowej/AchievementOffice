using AchievementOffice.Models;

namespace AchievementOffice.Services;

public interface IAchievementVerificationRequestService
{
    Task<Result<AchievementVerificationRequestResponse>>
        CreateAsync(
            Guid achievementId,
            Guid requesterUserId,
            CreateAchievementVerificationRequestDto request);

    Task<Result>
        ApproveAsync(
            Guid requestId,
            Guid userId);

    Task<Result>
        RejectAsync(
            Guid requestId,
            Guid userId);

    Task<Result<List<AchievementVerificationRequestResponse>>>
        GetPendingForUserAsync(Guid userId);

    Task<Result<List<UserReviewerResponse>>>
        GetAvailableReviewersAsync(Guid achievementId);
}