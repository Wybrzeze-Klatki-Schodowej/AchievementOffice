using AchievementOffice.Models;

namespace AchievementOffice.Services
{
    public interface IRankingService
    {
        Task<Result> ApplyAchievementPoints(Guid reactingUserId, Guid ownerId, bool? newState, bool? prevState);
        Task<Result> ApplyShoutOutPoints(Guid reactingUserId, Guid ownerId, bool addPoints);
        Task<Result> ApplyShoutOutPointsCreate(Guid reactingUserId, Guid ownerId, bool addPoints);
        Task<Result<List<UserRankingResponse>>> GetUserRanking();
        Task<Result<List<GroupRankingResponse>>> GetGroupRanking();
    }
}
