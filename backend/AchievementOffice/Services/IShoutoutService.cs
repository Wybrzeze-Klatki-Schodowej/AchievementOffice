using AchievementOffice.Models;

namespace AchievementOffice.Services
{
    public interface IShoutoutService
    {
        Task<ShoutoutResponseDto> CreateAsync(CreateShoutoutDto createDto, Guid senderId);
        Task<ShoutoutResponseDto?> UpdateAsync(Guid shoutoutId, UpdateShoutoutDto updateDto);
        Task<bool> DeleteAsync(Guid shoutoutId);
        Task<ShoutoutResponseDto?> GetShoutoutByIdAsync(Guid shoutoutId);
        Task<List<ShoutoutResponseDto>> GetAllShoutoutsAsync();
    }
}