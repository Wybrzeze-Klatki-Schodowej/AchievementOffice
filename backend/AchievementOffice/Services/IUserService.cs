using AchievementOffice.Models;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Services
{
    public interface IUserService
    {
        Task<LoginResult> LoginAsync(LoginRequest request);
        Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request);
        Task<List<UserDto>> GetAllUsersAsync2();
        
        

        Task<UserProfileResponse?> GetUserProfileAsync(Guid userId);

        Task<List<UserProfileResponse>> GetAllUsersAsync();
    }
}
