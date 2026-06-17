using AchievementOffice.Models;

namespace AchievementOffice.Services;

public interface IAdminService
{
    Task<List<AdminUserProfileResponse>> GetAllUsersAsync(bool? isActiveFilter = null);
    Task<AdminStatsResponse> GetStatsAsync();
    Task<Result> UpdateUserStatusAsync(Guid userId, bool isActive);
    Task<List<RankResponse>> GetRanksAsync();
    Task<Result> UpdateUserRankAsync(Guid userId, Guid? rankId);
    Task<Result> DeleteCommentAsync(Guid commentId);
    Task<Result> DeleteAchievementAsync(Guid achievementId);
    Task<Result> CreateRankAsync(CreateRankRequest request);
    Task<Result> DeleteShoutoutAsync(Guid shoutoutId);
}