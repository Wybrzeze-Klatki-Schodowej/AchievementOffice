using AchievementOffice.Models;

namespace AchievementOffice.Services;

public interface IAdminService
{
    Task<List<UserProfileResponse>> GetAllUsersAsync(bool? isActiveFilter=null);
    Task<AdminStatsResponse> GetStatsAsync();
    Task<Result> UpdateUserStatusAsync(Guid userId, bool isActive);
    Task<Result> DeleteUserAsync(Guid userId);

}