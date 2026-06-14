using AchievementOffice.Configuration;
using AchievementOffice.Data;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AchievementOffice.Services
{
    public class RankingService : IRankingService
    {
        private readonly AppDbContext _context;
        private readonly RankingSettings _rankingSettings;
        public RankingService(AppDbContext context, IOptions<RankingSettings> options)
        {
            _context = context;
            _rankingSettings = options.Value;
        }

        private async Task<Result> AddPoints(Guid reactingUserId, Guid ownerId, decimal value)
        {
            var reactingUser = await _context.Users.Include(u => u.Rank).FirstOrDefaultAsync(u => u.Id == reactingUserId);
            var owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == ownerId);

            if (reactingUser == null || owner == null) return Result.Fail("User have not been found");

            decimal toAdd = reactingUser.Rank?.Multiplier ?? 1.0m;

            owner.RankingPoints += value * toAdd;
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> AddPointsFromAchievement(Guid reactingUserId, Guid ownerId)
        {
            return await AddPoints(reactingUserId, ownerId, _rankingSettings.AchievementApprovedBasePoints);
        }

        public async Task<Result> AddPointsFromShoutOut(Guid reactingUserId, Guid ownerId)
        {
            return await AddPoints(reactingUserId, ownerId, _rankingSettings.ShoutoutBase);
        }

        public async Task<Result> SubtractPointsFromAchievement(Guid reactingUserId, Guid ownerId)
        {
            return await AddPoints(reactingUserId, ownerId, _rankingSettings.DisapprovalPoints);
        }

        

        public async Task<Result> UndoPointsFromAchievement(Guid reactingUserId, Guid ownerId)
        {
            return await AddPoints(reactingUserId, ownerId, -_rankingSettings.AchievementApprovedBasePoints);
        }

        public async Task<Result> UndoPointsFromShoutOut(Guid reactingUserId, Guid ownerId)
        {
            return await AddPoints(reactingUserId, ownerId, -_rankingSettings.ShoutoutBase);
        }

        public async Task<Result> UndoSubtractPointsFromAchievement(Guid reactingUserId, Guid ownerId)
        {
            return await AddPoints(reactingUserId, ownerId, -_rankingSettings.DisapprovalPoints);
        }

        public async Task<Result> AddPointsReactionS(Guid reactingUserId, Guid ownerId)
        {
            return await AddPoints(reactingUserId, ownerId, _rankingSettings.ReactionShoutOutPoints);
        }

        public async Task<Result> UndoPointsReactionS(Guid reactingUserId, Guid ownerId)
        {
            return await AddPoints(reactingUserId, ownerId, -_rankingSettings.ReactionShoutOutPoints);
        }
    }
}
