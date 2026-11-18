using AchievementOffice.Models;

namespace AchievementOffice.Services
{
    public enum RankingAction { Approval, Disapproval, Shoutout, Reaction }

    public interface IRankingService
    {
        Task<decimal> CalculateAwardAsync(Guid actorId, RankingAction action);
        Task ChangeScoreAsync(Guid ownerId, decimal delta);
        Task<Result<List<UserRankingResponse>>> GetUserRanking();
        Task<Result<List<GroupRankingResponse>>> GetGroupRanking();
    }
}
