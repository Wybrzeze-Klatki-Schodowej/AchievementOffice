using AchievementOffice.Models;

namespace AchievementOffice.Services;

public interface IUserService
{
    Task<LoginResult> LoginAsync(LoginRequest request);
    Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request);

    Task<UserProfileResponse?> GetUserProfileAsync(Guid userId);

    Task<List<UserProfileResponse>> GetAllUsersAsync();

    Task<OperationResult<UserProfileResponse>> UpdateUserAsync(
        Guid userId,
        UpdateUserRequest request
    );

    Task<OperationResult> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request
    );
}
