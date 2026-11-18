using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Services
{
    public class RankingService : IRankingService
    {
        private readonly AppDbContext _context;
        public RankingService(AppDbContext context)
        {
            _context = context;
        }

        // Stages score changes; the caller saves the action and score together.
        public async Task<decimal> CalculateAwardAsync(Guid actorId, RankingAction action)
        {
            var settings = await _context.RankingConfigurations.AsNoTracking().SingleAsync(c => c.Id == 1);
            var actor = await _context.Users.Include(u => u.Rank).SingleAsync(u => u.Id == actorId);
            var amount = action switch
            {
                RankingAction.Approval => settings.AchievementApprovedBasePoints,
                RankingAction.Disapproval => settings.DisapprovalPoints,
                RankingAction.Shoutout => settings.ShoutoutBase,
                RankingAction.Reaction => settings.ReactionShoutOutPoints,
                _ => throw new ArgumentOutOfRangeException(nameof(action))
            };
            return decimal.Round(amount * (actor.Rank?.Multiplier ?? 1m), 2, MidpointRounding.AwayFromZero);
        }

        public async Task ChangeScoreAsync(Guid ownerId, decimal delta)
        {
            var owner = await _context.Users.SingleAsync(u => u.Id == ownerId);
            owner.RankingPoints += delta;
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
