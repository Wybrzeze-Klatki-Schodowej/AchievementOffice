
using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Services
{
    public class ShoutoutService : IShoutoutService
    {
        private readonly AppDbContext _appDbContext;

        public ShoutoutService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<ShoutoutResponseDto> CreateAsync(CreateShoutoutDto createDto, Guid senderId)
        {
            var shoutout = new Shoutout
            {
                ShoutoutId = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = createDto.ReceiverId,
                Title = createDto.Title,
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _appDbContext.Shoutouts.Add(shoutout);
            await _appDbContext.SaveChangesAsync();

            await _appDbContext.Entry(shoutout).Reference(s => s.Sender).LoadAsync();
            await _appDbContext.Entry(shoutout.Sender).Reference(s => s.UserDetails).LoadAsync();
            await _appDbContext.Entry(shoutout).Reference(s => s.Receiver).LoadAsync();
            await _appDbContext.Entry(shoutout.Receiver).Reference(s => s.UserDetails).LoadAsync();

            return MapToDto(shoutout);
        }

        public async Task<ShoutoutResponseDto?> UpdateAsync(Guid shoutoutId, UpdateShoutoutDto updateDto)
        {
            var shoutout = await _appDbContext.Shoutouts
                .Include(s => s.Sender).ThenInclude(u => u.UserDetails)
                .Include(s => s.Receiver).ThenInclude(u => u.UserDetails)
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return null;

            shoutout.Title = updateDto.Title;
            shoutout.Description = updateDto.Description;
            shoutout.UpdatedAt = DateTime.UtcNow;

            await _appDbContext.SaveChangesAsync();
            return MapToDto(shoutout);
        }

        public async Task<bool> DeleteAsync(Guid shoutoutId)
        {
            var shoutout = await _appDbContext.Shoutouts
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return false;

            shoutout.DeletedAt = DateTime.UtcNow;
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ShoutoutResponseDto?> GetShoutoutByIdAsync(Guid shoutoutId)
        {
            var shoutout = await _appDbContext.Shoutouts
                .Include(s => s.Sender).ThenInclude(u => u.UserDetails)
                .Include(s => s.Receiver).ThenInclude(u => u.UserDetails)
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return null;

            return MapToDto(shoutout);
        }

        public async Task<List<ShoutoutResponseDto>> GetAllShoutoutsAsync()
        {
            var shoutouts = await _appDbContext.Shoutouts
                .Include(s => s.Sender).ThenInclude(u => u.UserDetails)
                .Include(s => s.Receiver).ThenInclude(u => u.UserDetails)
                .Where(s => s.DeletedAt == null)
                .ToListAsync();

            return shoutouts.Select(MapToDto).ToList();
        }

        private static ShoutoutResponseDto MapToDto(Shoutout shoutout)
        {
            return new ShoutoutResponseDto
            {
                ShoutoutId = shoutout.ShoutoutId,
                SenderId = shoutout.SenderId,
                SenderLogin = shoutout.Sender?.Login ?? "Unknown",
                SenderFirstname = shoutout.Sender?.UserDetails?.Firstname ?? "",
                SenderLastname = shoutout.Sender?.UserDetails?.Lastname ?? "",
                ReceiverId = shoutout.ReceiverId,
                ReceiverLogin = shoutout.Receiver?.Login ?? "Unknown",
                ReceiverFirstname = shoutout.Receiver?.UserDetails?.Firstname ?? "",
                ReceiverLastname = shoutout.Receiver?.UserDetails?.Lastname ?? "",
                Title = shoutout.Title,
                Description = shoutout.Description,
                CreatedAt = shoutout.CreatedAt,
                UpdatedAt = shoutout.UpdatedAt
            };
        }
    }
}
