using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AchievementOffice.Services
{
    public class ShoutoutService : IShoutoutService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoutoutService(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<ShoutoutResponse>> CreateAsync(CreateShoutoutRequest createDto)
        {
            if (createDto.ReceiverId == GetUserId())
                return Result<ShoutoutResponse>.Fail("Cannot send shoutout to yourself");

            if (GetUserId() == Guid.Empty)
                return Result<ShoutoutResponse>.Fail("Unauthorized");

            var shoutout = new Shoutout
            {
                ShoutoutId = Guid.NewGuid(),
                SenderId = GetUserId(),
                ReceiverId = createDto.ReceiverId,
                Title = createDto.Title,
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _appDbContext.Shoutouts.Add(shoutout);
            await _appDbContext.SaveChangesAsync();

            return Result<ShoutoutResponse>.Success(MapToDto(shoutout));
        }

        public async Task<Result<ShoutoutResponse>> UpdateAsync(Guid shoutoutId, UpdateShoutoutRequest updateDto)
        {
            var shoutout = await _appDbContext.Shoutouts
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return Result<ShoutoutResponse>.Fail("Not found");

            var userId = GetUserId();
            var role = GetRole();

            if (GetUserId() == Guid.Empty)
                return Result<ShoutoutResponse>.Fail("Unauthorized");

            var isOwner = shoutout.SenderId == userId;
            var isAdmin = role == "Admin";

            if (!isOwner && !isAdmin)
                return Result<ShoutoutResponse>.Fail("Forbidden");

            shoutout.Title = updateDto.Title;
            shoutout.Description = updateDto.Description;
            shoutout.UpdatedAt = DateTime.UtcNow;

            await _appDbContext.SaveChangesAsync();

            return Result<ShoutoutResponse>.Success(MapToDto(shoutout));
        }

        public async Task<Result<bool>> DeleteAsync(Guid shoutoutId)
        {
            var shoutout = await _appDbContext.Shoutouts
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return Result<bool>.Fail("Not found");

            var userId = GetUserId();
            var role = GetRole();

            if (GetUserId() == Guid.Empty)
                return Result<bool>.Fail("Unauthorized");

            var isOwner = shoutout.SenderId == userId;
            var isAdmin = role == "Admin";

            if (!isOwner && !isAdmin)
                return Result<bool>.Fail("Forbidden");

            shoutout.DeletedAt = DateTime.UtcNow;
            await _appDbContext.SaveChangesAsync();
            
            return Result<bool>.Success(true);
        }

        public async Task<Result<ShoutoutResponse>> GetShoutoutByIdAsync(Guid shoutoutId)
        {
            var userId = GetUserId();
            var shoutout = await _appDbContext.Shoutouts
                .Include(s => s.Sender).ThenInclude(u => u.UserDetails)
                .Include(s => s.Receiver).ThenInclude(u => u.UserDetails)
                .Include(s => s.Kudos)
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return Result<ShoutoutResponse>.Fail("Not found");

            return Result<ShoutoutResponse>.Success(MapToDto(shoutout));
        }

        public async Task<Result<List<ShoutoutResponse>>> GetAllShoutoutsAsync()
        {
            var userId = GetUserId();
            var shoutouts = await _appDbContext.Shoutouts
                .Include(s => s.Sender).ThenInclude(u => u.UserDetails)
                .Include(s => s.Receiver).ThenInclude(u => u.UserDetails)
                .Include(s => s.Kudos)
                .Where(s => s.DeletedAt == null)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var result = shoutouts.Select(s => MapToDto(s)).ToList();
            return Result<List<ShoutoutResponse>>.Success(result);
        }

        public async Task<Result<ShoutoutResponse>> ReactAsync(Guid shoutoutId, ReactionType reaction)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Result<ShoutoutResponse>.Fail("Unauthorized");

            var shoutout = await _appDbContext.Shoutouts
                .Include(s => s.Kudos)
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null) return Result<ShoutoutResponse>.Fail("Not found");

            var existingReaction = await _appDbContext.KudosShoutouts
                .FirstOrDefaultAsync(r => r.ShoutoutId == shoutoutId && r.UserId == userId);

            if (existingReaction != null)
            {
                if (existingReaction.Reaction == reaction)
                {
                    // If same reaction, remove it (toggle behavior)
                    _appDbContext.KudosShoutouts.Remove(existingReaction);
                }
                else
                {
                    existingReaction.Reaction = reaction;
                }
            }
            else
            {
                var newReaction = new KudosShoutout
                {
                    ShoutoutId = shoutoutId,
                    UserId = userId,
                    Reaction = reaction
                };
                _appDbContext.KudosShoutouts.Add(newReaction);
            }

            await _appDbContext.SaveChangesAsync();
            return await GetShoutoutByIdAsync(shoutoutId);
        }

        public async Task<Result<ShoutoutResponse>> UnreactAsync(Guid shoutoutId)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Result<ShoutoutResponse>.Fail("Unauthorized");

            var reaction = await _appDbContext.KudosShoutouts
                .FirstOrDefaultAsync(r => r.ShoutoutId == shoutoutId && r.UserId == userId);

            if (reaction != null)
            {
                _appDbContext.KudosShoutouts.Remove(reaction);
                await _appDbContext.SaveChangesAsync();
            }

            return await GetShoutoutByIdAsync(shoutoutId);
        }

        private ShoutoutResponse MapToDto(Shoutout shoutout)
        {
            var currentUserId = GetUserId();
            var response = new ShoutoutResponse
            {
                ShoutoutId = shoutout.ShoutoutId,
                SenderId = shoutout.SenderId,
                SenderLogin = shoutout.Sender?.Login ?? string.Empty,
                SenderFirstname = shoutout.Sender?.UserDetails?.Firstname ?? string.Empty,
                SenderLastname = shoutout.Sender?.UserDetails?.Lastname ?? string.Empty,
                ReceiverId = shoutout.ReceiverId,
                ReceiverLogin = shoutout.Receiver?.Login ?? string.Empty,
                ReceiverFirstname = shoutout.Receiver?.UserDetails?.Firstname ?? string.Empty,
                ReceiverLastname = shoutout.Receiver?.UserDetails?.Lastname ?? string.Empty,
                Title = shoutout.Title,
                Description = shoutout.Description,
                CreatedAt = shoutout.CreatedAt,
                UpdatedAt = shoutout.UpdatedAt
            };

            if (shoutout.Kudos != null)
            {
                response.Reactions = shoutout.Kudos
                    .GroupBy(k => k.Reaction)
                    .ToDictionary(g => GetEmoji(g.Key), g => g.Count());

                var userReaction = shoutout.Kudos.FirstOrDefault(k => k.UserId == currentUserId);
                if (userReaction != null)
                {
                    response.CurrentUserReaction = GetEmoji(userReaction.Reaction);
                }
            }

            return response;
        }

        private static string GetEmoji(ReactionType reaction)
        {
            return reaction switch
            {
                ReactionType.Like => "👍",
                ReactionType.Love => "❤️",
                ReactionType.Haha => "😂",
                ReactionType.Wow => "😮",
                ReactionType.Sad => "😢",
                ReactionType.Angry => "😠",
                _ => "❓"
            };
        }

        private Guid GetUserId()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return Guid.Empty;
            var claim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
        
        private string? GetRole()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        }
    }
}
