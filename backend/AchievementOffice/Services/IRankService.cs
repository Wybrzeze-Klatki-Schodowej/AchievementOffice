using AchievementOffice.Entities;
using AchievementOffice.Models;

namespace AchievementOffice.Services
{
    public interface IRankService
    {
        Task<Result<List<RankResponse>>> GetAllRanks();
        Task<Result<RankResponse>> GetRank(Guid rankId);
        Task<Result<RankResponse>> CreateRank(CreateRankRequest dto);
        Task<Result> DeleteRank(Guid rankId);
        Task<Result<RankResponse>> UpdateRank(Guid rankId, RankRequest dto);
        Task<Result<List<UserResponse>>> GetAllUsersWithRank(Guid rankId);
    }
}
