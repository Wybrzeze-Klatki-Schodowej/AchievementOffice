using AchievementOffice.Models;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Services;

public interface IUserService
{
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
}
