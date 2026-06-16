using AchievementOffice.Configuration;
using AchievementOffice.Data;
using AchievementOffice.Entities;
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

        public async Task<Result> ApplyAchievementPoints(Guid reactingUserId, Guid ownerId, bool? prevState, bool? newState)
        {
            if (newState == prevState)
                return Result.Success();

            decimal delta = 0m;

            if (prevState == true)
                delta -= _rankingSettings.AchievementApprovedBasePoints;
            else if (prevState == false)
                delta -= _rankingSettings.DisapprovalPoints;

            if (newState == true)
                delta += _rankingSettings.AchievementApprovedBasePoints;
            else if (newState == false)
                delta += _rankingSettings.DisapprovalPoints;

            if (delta == 0)
                return Result.Success();

            return await AddPoints(reactingUserId, ownerId, delta);
        }

        public async Task<Result> ApplyShoutOutPoints(Guid reactingUserId, Guid ownerId, bool addPoints)
        {
            if (addPoints)
                return await AddPoints(reactingUserId, ownerId, _rankingSettings.ReactionShoutOutPoints);
            return await AddPoints(reactingUserId, ownerId, -_rankingSettings.ReactionShoutOutPoints);
        }

        public async Task<Result> ApplyShoutOutPointsCreate(Guid reactingUserId, Guid ownerId, bool addPoints)
        {
            if (addPoints)
                return await AddPoints(reactingUserId, ownerId, _rankingSettings.ShoutoutBase);
            return await AddPoints(reactingUserId, ownerId, -_rankingSettings.ShoutoutBase);
        }

        public async Task<Result<List<UserRankingResponse>>> GetUserRanking()
        {
            var usersEntities = await _context.Users
                .Include(u => u.UserDetails)
                .Where(u => u.DeletedAt == null)
                .OrderByDescending(u => u.RankingPoints)
                .ToListAsync();

            var users = usersEntities.
                Select(u => MapToDto(u))
                .ToList();

            return Result<List<UserRankingResponse>>.Success(users);
        }

        public async Task<Result<List<GroupRankingResponse>>> GetGroupRanking()
        {
            var groupsEntity = await _context.Groups
                .Where(g => g.DeletedAt == null)
                .Include(g => g.GroupUsers)
                .ThenInclude(gu => gu.User)
                .ToListAsync();

            var groups = groupsEntity
                .Select(g => MapToDto(g, g.GroupUsers.Sum(gu => gu.User.RankingPoints)))
                .OrderByDescending(g => g.RankingPoints)
                .ToList();

            return Result<List<GroupRankingResponse>>.Success(groups);
        }

        private UserRankingResponse MapToDto(User user)
        {
            return new UserRankingResponse
            {
                UserId = user.Id,
                UserName = user.Login,
                Firstname = user.UserDetails?.Firstname ?? string.Empty,
                Lastname = user.UserDetails?.Lastname ?? string.Empty,
                Avatar = user.UserDetails?.AvatarUrl,
                RankingPoints = user.RankingPoints
            };
        }

        private GroupRankingResponse MapToDto(Group group, decimal points)
        {
            return new GroupRankingResponse
            {
                GroupId = group.GroupId,
                GroupName = group.Name,
                Avatar = group.AvatarUrl,
                RankingPoints = points
            };
        }
    }
}
