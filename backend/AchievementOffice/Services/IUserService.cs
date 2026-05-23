using AchievementOffice.Models;

namespace AchievementOffice.Services;

public interface IUserService
{
    Task<LoginResult> LoginAsync(LoginRequest request);
    Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request);

    Task<UserProfileResponse?> GetUserProfileAsync(Guid userId);

    Task<List<UserProfileResponse>> GetAllUsersAsync();

    Task<UserProfileResponse?> UpdateUserAsync(
        Guid userId,
        UpdateUserRequest request
    );

    Task<bool> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request
    );
}
