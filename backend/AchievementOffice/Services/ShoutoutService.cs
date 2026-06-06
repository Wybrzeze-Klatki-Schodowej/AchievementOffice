
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

            //await _appDbContext.Entry(shoutout).Reference(s => s.Sender).LoadAsync();
            //await _appDbContext.Entry(shoutout.Sender).Reference(s => s.UserDetails).LoadAsync();
            //await _appDbContext.Entry(shoutout).Reference(s => s.Receiver).LoadAsync();
            //await _appDbContext.Entry(shoutout.Receiver).Reference(s => s.UserDetails).LoadAsync();

            return Result<ShoutoutResponse>.Success(MapToDto(shoutout));
        }

        public async Task<Result<ShoutoutResponse>> UpdateAsync(Guid shoutoutId, UpdateShoutoutRequest updateDto)
        {
            var shoutout = await _appDbContext.Shoutouts
                //.Include(s => s.Sender).ThenInclude(u => u.UserDetails)
                //.Include(s => s.Receiver).ThenInclude(u => u.UserDetails)
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return Result<ShoutoutResponse>.Fail("Shoutout not found");

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
                return Result<bool>.Fail("Shoutout not found");

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
            var shoutout = await _appDbContext.Shoutouts
                //.Include(s => s.Sender).ThenInclude(u => u.UserDetails)
                //.Include(s => s.Receiver).ThenInclude(u => u.UserDetails)
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return Result<ShoutoutResponse>.Fail("Shoutout not found");

            return Result<ShoutoutResponse>.Success(MapToDto(shoutout));
        }

        public async Task<Result<List<ShoutoutResponse>>> GetAllShoutoutsAsync()
        {
            var shoutouts = await _appDbContext.Shoutouts
                //.Include(s => s.Sender).ThenInclude(u => u.UserDetails)
                //.Include(s => s.Receiver).ThenInclude(u => u.UserDetails)
                .Where(s => s.DeletedAt == null)
                .ToListAsync();

            var result =shoutouts.Select(MapToDto).ToList();
            return Result<List<ShoutoutResponse>>.Success(result);
        }

        private static ShoutoutResponse MapToDto(Shoutout shoutout)
        {
            return new ShoutoutResponse
            {
                ShoutoutId = shoutout.ShoutoutId,
                SenderId = shoutout.SenderId,
                //SenderLogin = shoutout.Sender?.Login ?? "Unknown",
                //SenderFirstname = shoutout.Sender?.UserDetails?.Firstname ?? "",
                //SenderLastname = shoutout.Sender?.UserDetails?.Lastname ?? "",
                ReceiverId = shoutout.ReceiverId,
                //ReceiverLogin = shoutout.Receiver?.Login ?? "Unknown",
                //ReceiverFirstname = shoutout.Receiver?.UserDetails?.Firstname ?? "",
                //ReceiverLastname = shoutout.Receiver?.UserDetails?.Lastname ?? "",
                Title = shoutout.Title,
                Description = shoutout.Description,
                CreatedAt = shoutout.CreatedAt,
                UpdatedAt = shoutout.UpdatedAt
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
