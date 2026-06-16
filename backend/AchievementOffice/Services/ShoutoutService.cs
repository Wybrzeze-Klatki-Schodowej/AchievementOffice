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
            var userId = GetUserId();

            if (userId == Guid.Empty)
                return Result<ShoutoutResponse>.Fail("Unauthorized");

            if (createDto.ReceiverId == userId)
                return Result<ShoutoutResponse>.Fail("Cannot send shoutout to yourself");

            if (string.IsNullOrWhiteSpace(createDto.Title))
                return Result<ShoutoutResponse>.Fail("Title is required");

            var shoutout = new Shoutout
            {
                ShoutoutId = Guid.NewGuid(),
                SenderId = userId,
                ReceiverId = createDto.ReceiverId,
                Title = createDto.Title,
                Description = createDto.Description,
                VisibilityId = createDto.VisibilityId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _appDbContext.Shoutouts.Add(shoutout);

            if (createDto.VisibilityId == (int)VisibilityMode.Group)
            {
                if (createDto.GroupIds == null || !createDto.GroupIds.Any())
                {
                    return Result<ShoutoutResponse>.Fail(
                        "At least one group is required when visibility is set to Groups");
                }

                if (createDto.GroupIds?.Any() == true)
                {
                    var shoutoutGroups = createDto.GroupIds.Select(groupId => new ShoutoutGroup
                    {
                        ShoutoutId = shoutout.ShoutoutId,
                        GroupId = groupId
                    });

                    _appDbContext.ShoutoutGroups.AddRange(shoutoutGroups);
                }
            }
            else if (createDto.GroupIds?.Any() == true)
            {
                return Result<ShoutoutResponse>.Fail(
                    "GroupIds can only be provided when visibility is set to Groups");
            }

            await _appDbContext.SaveChangesAsync();

            var createdShoutout = await _appDbContext.Shoutouts
                .Include(s => s.Sender)
                    .ThenInclude(u => u.UserDetails)
                .Include(s => s.Receiver)
                    .ThenInclude(u => u.UserDetails)
                .Include(s => s.Kudos)
                .FirstAsync(s => s.ShoutoutId == shoutout.ShoutoutId);

            return Result<ShoutoutResponse>.Success(MapToDto(shoutout));
        }

        public async Task<Result<ShoutoutResponse>> UpdateAsync(Guid shoutoutId, UpdateShoutoutRequest updateDto)
        {
            var shoutout = await _appDbContext.Shoutouts
                .Include(s => s.ShoutoutGroups)
                .Include(s => s.Sender)
                    .ThenInclude(u => u.UserDetails)
                .Include(s => s.Receiver)
                    .ThenInclude(u => u.UserDetails)
                .Include(s => s.Kudos)
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return Result<ShoutoutResponse>.Fail("Not found");

            var userId = GetUserId();
            var role = GetRole();

            if (userId == Guid.Empty)
                return Result<ShoutoutResponse>.Fail("Unauthorized");

            var isOwner = shoutout.SenderId == userId;
            var isAdmin = role == "Admin";

            if (!isOwner && !isAdmin)
                return Result<ShoutoutResponse>.Fail("Forbidden");

            shoutout.Title = updateDto.Title;
            shoutout.Description = updateDto.Description;
            shoutout.VisibilityId = updateDto.VisibilityId;
            shoutout.UpdatedAt = DateTime.UtcNow;

            _appDbContext.ShoutoutGroups.RemoveRange(shoutout.ShoutoutGroups);

            if (updateDto.VisibilityId == (int)VisibilityMode.Group)
            {
                if (updateDto.GroupIds == null || !updateDto.GroupIds.Any())
                {
                    return Result<ShoutoutResponse>.Fail(
                        "At least one group is required when visibility is set to Groups");
                }

                var shoutoutGroups = updateDto.GroupIds
                    .Distinct()
                    .Select(groupId => new ShoutoutGroup
                    {
                        ShoutoutId = shoutout.ShoutoutId,
                        GroupId = groupId
                    });

                _appDbContext.ShoutoutGroups.AddRange(shoutoutGroups);
            }
            else if (updateDto.GroupIds?.Any() == true)
            {
                return Result<ShoutoutResponse>.Fail(
                    "GroupIds can only be provided when visibility is set to Groups");
            }


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

            if (userId == Guid.Empty)
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
                .Include(s => s.ShoutoutGroups)
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return Result<ShoutoutResponse>.Fail("Not found");
            
            var role = GetRole();
            var isAdmin = role == "Admin";

            var hasAccess =
                isAdmin ||
                shoutout.SenderId == userId ||
                shoutout.ReceiverId == userId ||
                shoutout.VisibilityId == (int)VisibilityMode.Public ||
                (shoutout.VisibilityId == (int)VisibilityMode.Group && IsUserInShoutoutGroups(shoutout, userId));

            if (hasAccess)
            {
                return Result<ShoutoutResponse>.Success(MapToDto(shoutout));
            }

            return Result<ShoutoutResponse>.Fail("Forbidden");
        }

        public async Task<Result<List<ShoutoutResponse>>> GetAllShoutoutsAsync()
        {
            var userId = GetUserId();
            var role = GetRole();
            var isAdmin = role == "Admin";

            var query = _appDbContext.Shoutouts
                .AsQueryable()
                .Where(s => s.DeletedAt == null);

            if (!isAdmin)
            {
                query = query.Where(s =>
                    s.SenderId == userId ||
                    s.ReceiverId == userId ||
                    s.VisibilityId == (int)VisibilityMode.Public ||
                    (s.VisibilityId == (int)VisibilityMode.Group &&
                        s.ShoutoutGroups.Any(g =>
                            _appDbContext.GroupUsers.Any(ug =>
                                ug.UserId == userId &&
                                ug.GroupId == g.GroupId
                            )
                        )
                    )
                );
            }

            var shoutouts = await query
                .Include(s => s.Sender).ThenInclude(u => u.UserDetails)
                .Include(s => s.Receiver).ThenInclude(u => u.UserDetails)
                .Include(s => s.Kudos)
                .Include(s => s.ShoutoutGroups)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var result = shoutouts.Select(MapToDto).ToList();
            return Result<List<ShoutoutResponse>>.Success(result);
        }

        private bool IsUserInShoutoutGroups(Shoutout shoutout, Guid userId)
        {
            var shoutoutGroupIds = shoutout.ShoutoutGroups
                .Select(g => g.GroupId)
                .ToList();

            if (!shoutoutGroupIds.Any())
                return false;

            var userGroupIds = _appDbContext.GroupUsers
                .Where(ug => ug.UserId == userId)
                .Select(ug => ug.GroupId)
                .ToList();

            return shoutoutGroupIds.Intersect(userGroupIds).Any();
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

            bool? prevState = existingReaction != null ? true : null;

            if (existingReaction != null)
            {
                if (existingReaction.Reaction == reaction)
                {
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

            var result = await GetShoutoutByIdAsync(shoutoutId);

            if (result.IsSuccess && result.Value != null)
                result.Value.PrevReactionState = prevState;

            return result;
        }

        public async Task<Result<ShoutoutResponse>> UnreactAsync(Guid shoutoutId)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Result<ShoutoutResponse>.Fail("Unauthorized");

            var reaction = await _appDbContext.KudosShoutouts
                .FirstOrDefaultAsync(r => r.ShoutoutId == shoutoutId && r.UserId == userId);

            bool? prevState = reaction != null ? true : null;

            if (reaction != null)
            {
                _appDbContext.KudosShoutouts.Remove(reaction);
                await _appDbContext.SaveChangesAsync();
            }

            var result = await GetShoutoutByIdAsync(shoutoutId);
            if (result.IsSuccess && result.Value != null)
                result.Value.PrevReactionState = prevState;

            return result;
        }

        public async Task<Result<List<ShoutoutResponse>>> GetReceivedShoutoutsAsync(Guid receiverId)
        {
            var shoutouts = await _appDbContext.Shoutouts
                .Include(s => s.Sender)
                    .ThenInclude(u => u.UserDetails)
                .Include(s => s.Receiver)
                    .ThenInclude(u => u.UserDetails)
                .Include(s => s.Kudos)
                .Where(s =>
                    s.DeletedAt == null &&
                    s.ReceiverId == receiverId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var result = shoutouts
                .Select(MapToDto)
                .ToList();

            return Result<List<ShoutoutResponse>>.Success(result);
        }

        private ShoutoutResponse MapToDto(Shoutout shoutout)
        {
            var currentUserId = GetUserId();
            var response = new ShoutoutResponse
            {
                ShoutoutId = shoutout.ShoutoutId,
                SenderId = shoutout.SenderId,
                SenderLogin = shoutout.Sender.Login,
                SenderFirstname = shoutout.Sender.UserDetails.Firstname,
                SenderLastname = shoutout.Sender.UserDetails.Lastname,
                ReceiverId = shoutout.ReceiverId,
                ReceiverLogin = shoutout.Receiver.Login,
                ReceiverFirstname = shoutout.Receiver.UserDetails.Firstname,
                ReceiverLastname = shoutout.Receiver.UserDetails.Lastname,
                Title = shoutout.Title,
                Description = shoutout.Description,
                VisibilityId = shoutout.VisibilityId,
                GroupIds = shoutout.ShoutoutGroups?
                    .Select(x => x.GroupId)
                    .ToList() ?? [],
                CreatedAt = shoutout.CreatedAt,
                UpdatedAt = shoutout.UpdatedAt,
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
