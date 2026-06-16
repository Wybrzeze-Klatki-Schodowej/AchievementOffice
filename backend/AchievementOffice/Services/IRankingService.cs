using AchievementOffice.Models;

namespace AchievementOffice.Services
{
    public interface IRankingService
    {
        Task<Result> ApplyShoutOutPoints(Guid reactingUserId, Guid ownerId, bool? isApproved, bool dtoApproved);
        Task<Result> ApplypAchievementPoints(Guid reactingUserId, Guid ownerId, bool? isApproved, bool dtoApproved);
    }
}
