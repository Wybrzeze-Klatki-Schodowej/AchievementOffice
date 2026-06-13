using AchievementOffice.Models;

namespace AchievementOffice.Services
{
    public interface IRankingService
    {
        Task<Result> AddPointsFromShoutOut(Guid reactingUserId, Guid ownerId);
        Task<Result> AddPointsFromAchievement(Guid reactingUserId, Guid ownerId);
        Task<Result> SubtractPointsFromAchievement(Guid reactingUserId, Guid ownerId);
        Task<Result> UndoPointsFromShoutOut(Guid reactingUserId, Guid ownerId);
        Task<Result> UndoPointsFromAchievement(Guid reactingUserId, Guid ownerId);
        Task<Result> UndoSubtractPointsFromAchievement(Guid reactingUserId, Guid ownerId);
    }
}
