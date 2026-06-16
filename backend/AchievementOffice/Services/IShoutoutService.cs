using AchievementOffice.Models;
using AchievementOffice.Entities;

namespace AchievementOffice.Services
{
    public interface IShoutoutService
    {
        Task<Result<ShoutoutResponse>> CreateAsync(CreateShoutoutRequest createDto);
        Task<Result<ShoutoutResponse>> UpdateAsync(Guid shoutoutId, UpdateShoutoutRequest updateDto);
        Task<Result<bool>> DeleteAsync(Guid shoutoutId);
        Task<Result<ShoutoutResponse>> GetShoutoutByIdAsync(Guid shoutoutId);
        Task<Result<List<ShoutoutResponse>>> GetAllShoutoutsAsync();
        Task<Result<ShoutoutResponse>> ReactAsync(Guid shoutoutId, ReactionType reaction);
        Task<Result<ShoutoutResponse>> UnreactAsync(Guid shoutoutId);

        Task<Result<List<ShoutoutResponse>>> GetReceivedShoutoutsAsync(Guid receiverId);
    }
}