using AchievementOffice.Models;

namespace AchievementOffice.Services;

public interface IUserService
{
<<<<<<< HEAD
    Task<LoginResult> LoginAsync(LoginRequest request);
    Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request);

    Task<UserProfileResponse?> GetUserProfileAsync(Guid userId);

    Task<List<UserProfileResponse>> GetAllUsersAsync();

    Task<Result<UserProfileResponse>> UpdateUserAsync(
        Guid userId,
        UpdateUserRequest request
    );

    Task<Result> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request
    );
=======
    public interface IUserService
    {
        Task<LoginResult> LoginAsync(LoginRequest request);
        Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request);
        Task<List<UserDto>> GetAllUsersAsync();
    }
>>>>>>> d4df33e ([AOF-18] Added basic possibility to manage shoutouts from the frontend)
}
