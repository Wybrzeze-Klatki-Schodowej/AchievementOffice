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

        public async Task<ShoutoutResponseDto> CreateAsync(CreateShoutoutDto createDto)
        {
            var shoutout = new Shoutout
            {
                ShoutoutId = Guid.NewGuid(),
                SenderId = createDto.SenderId,
                ReceiverId = createDto.ReceiverId,
                Title = createDto.Title,
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _appDbContext.Shoutouts.Add(shoutout);
            await _appDbContext.SaveChangesAsync();

            return MapToDto(shoutout);
        }

        public async Task<ShoutoutResponseDto?> UpdateAsync(Guid shoutoutId, UpdateShoutoutDto updateDto)
        {
            var shoutout = await _appDbContext.Shoutouts
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
                .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);

            if (shoutout == null)
                return null;

            return MapToDto(shoutout);
        }

        public async Task<List<ShoutoutResponseDto>> GetAllShoutoutsAsync()
        {
            var shoutouts = await _appDbContext.Shoutouts
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
                ReceiverId = shoutout.ReceiverId,
                Title = shoutout.Title,
                Description = shoutout.Description,
                CreatedAt = shoutout.CreatedAt
            };
        }
    }
}