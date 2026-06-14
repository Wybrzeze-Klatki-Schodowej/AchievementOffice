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

        public async Task<Result> ApplypAchievementPoints(Guid reactingUserId, Guid ownerId, bool? isApproved, bool dtoApproved)
        {
            if (isApproved == true)
                return await AddPoints(reactingUserId, ownerId, _rankingSettings.AchievementApprovedBasePoints);
            else if (isApproved == false)
                return await AddPoints(reactingUserId, ownerId, _rankingSettings.DisapprovalPoints);

            if (dtoApproved == true)
                return await AddPoints(reactingUserId, ownerId, -_rankingSettings.AchievementApprovedBasePoints);
            return await AddPoints(reactingUserId, ownerId, -_rankingSettings.DisapprovalPoints);
        }

        public Task<Result> ApplyShoutOutPoints(Guid reactingUserId, Guid ownerId, bool? isApproved, bool dtoApproved)
        {
            throw new NotImplementedException();
        }
    }
}
