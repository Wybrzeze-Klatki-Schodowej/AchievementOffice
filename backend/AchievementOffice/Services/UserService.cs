using System.Text.RegularExpressions;
using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AchievementOffice.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.Include(u => u.UserRole).Include(u => u.UserDetails).FirstOrDefaultAsync(u => u.Login == request.Login);

        if (user == null)
            return new LoginResult() { IsSuccessful = false, ErrorMessage = "Invalid login or password" };

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!isPasswordValid)
            return new LoginResult() { IsSuccessful = false, ErrorMessage = "Invalid login or password" };

        if(user.DeletedAt != null)
            return new LoginResult() { IsSuccessful = false, ErrorMessage = "User account is deleted" };
        
        if(!user.IsActive)
            return new LoginResult() { IsSuccessful = false, ErrorMessage = "User account is inactive" };

        var token = _tokenService.GenerateToken(user);

        if (string.IsNullOrEmpty(token))
            return new LoginResult() { IsSuccessful = false };

        return new LoginResult() { IsSuccessful = true, Token = token };
    }

    public async Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request)
    {
        bool userExists = await _context.Users.AnyAsync(u => u.Email == request.Email || u.Login == request.Username);

        if (userExists)
            return new UserRegistrationResult() { IsSuccessful = false, ErrorMessage = "User already exists" };

        if (string.IsNullOrEmpty(request.RoleName))
            request.RoleName = "user";

        var userRole = await _context.UserRoles.FirstOrDefaultAsync(r => r.Name == request.RoleName);

        if (userRole == null)
            return new UserRegistrationResult() { IsSuccessful = false, ErrorMessage = $"User role {request.RoleName} does not exist" };

        var userDetails = new UserDetails()
        {
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            JobTitle = request.JobTitle
        };

        var user = new User()
        {
            Login = request.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Email = request.Email,
            UserDetails = userDetails,
            UserRoleId = userRole.Id,
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserRegistrationResult() { IsSuccessful = true };
    }

    public async Task<UserProfileResponse?> GetUserProfileAsync(Guid userId)
    {
        var requestingUserId = GetUserId();
        var requestingUserRole = GetRole();

        _logger.LogInformation(
            "[GetUserProfile] Target={TargetUserId}, Requester={RequestingUserId}, Role={Role}",
            userId, requestingUserId, requestingUserRole ?? "(null)");

        var user = await _context.Users
            .Include(u => u.UserDetails)
                .ThenInclude(ud => ud.ProfileGroups)
            .Include(u => u.UserRole)
            .Where(u => u.Id == userId)
//             .Select(u => new UserProfileResponse
//             {
//                 UserId = u.Id,
//                 Login = u.Login,
//                 Email = u.Email,
//                 FirstName = u.UserDetails.Firstname,
//                 LastName = u.UserDetails.Lastname,
//                 JobTitle = u.UserDetails.JobTitle,
//                 IsActive = u.IsActive,
//                 Bio = u.UserDetails.Bio,
//                 AvatarUrl = u.UserDetails.AvatarUrl,
//                 Role = u.UserRole.Name,
//                 CreatedAt = u.CreatedAt,
//                 UpdatedAt = u.UpdatedAt,
//                 RankingPoints = u.RankingPoints
//             })
            .FirstOrDefaultAsync();

        if (user == null)
            return null;

        var allowedGroupIds = user.UserDetails.ProfileGroups.Select(pg => pg.GroupId).ToList();
        
        bool isOwnProfile = requestingUserId == userId;
        bool isAdmin = requestingUserRole == "Admin";
        bool isGroupMember = false;

        if (user.UserDetails.VisibilityId == 3) // Group
        {
            isGroupMember = await _context.GroupUsers
                .AnyAsync(gu => gu.UserId == requestingUserId && allowedGroupIds.Contains(gu.GroupId));
        }

        bool canViewProfileMeta = isAdmin || isOwnProfile || user.UserDetails.VisibilityId == 1 || (user.UserDetails.VisibilityId == 3 && isGroupMember);

        _logger.LogInformation(
            "[GetUserProfile] VisibilityId={Visibility}, isOwnProfile={OwnProfile}, isAdmin={Admin}, isGroupMember={GroupMember}, canViewProfileMeta={CanView}, IsProfileRestricted={Restricted}",
            user.UserDetails.VisibilityId, isOwnProfile, isAdmin, isGroupMember, canViewProfileMeta, !canViewProfileMeta);

        return new UserProfileResponse
        {
            UserId = user.Id,
            Login = user.Login,
            Email = canViewProfileMeta ? user.Email : "",
            FirstName = user.UserDetails.Firstname,
            LastName = user.UserDetails.Lastname,
            JobTitle = canViewProfileMeta ? user.UserDetails.JobTitle : "",
            IsActive = user.IsActive,
            Bio = canViewProfileMeta ? user.UserDetails.Bio : null,
            AvatarUrl = user.UserDetails.AvatarUrl,
            Role = user.UserRole.Name,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            VisibilityId = user.UserDetails.VisibilityId,
            GroupIds = allowedGroupIds,
            IsProfileRestricted = !canViewProfileMeta,
            RankingPoints = user.RankingPoints
        };
    }

    public async Task<List<UserProfileResponse>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Include(u => u.UserDetails)
            .Include(u => u.UserRole)
            .Where(u => u.DeletedAt == null)
            .Select(u => new UserProfileResponse
            {
                UserId = u.Id,
                Login = u.Login,
                Email = u.Email,
                FirstName = u.UserDetails.Firstname,
                LastName = u.UserDetails.Lastname,
                JobTitle = u.UserDetails.JobTitle,
                IsActive = u.IsActive,
                Bio = u.UserDetails.Bio,
                AvatarUrl = u.UserDetails.AvatarUrl,
                Role = u.UserRole.Name,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                IsProfileRestricted = false, //?
                RankingPoints = u.RankingPoints
            })
            .ToListAsync();

        return users;
    }

    public async Task<Result<UserProfileResponse>> UpdateUserAsync(
        Guid userId,
        UpdateUserRequest request)
    {
        var user = await _context.Users
            .Include(u => u.UserDetails)
                .ThenInclude(ud => ud.ProfileGroups)
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(
                u => u.Id == userId &&
                    u.DeletedAt == null
            );

        if (user == null)
        {
            return Result<UserProfileResponse>
                .Fail("User not found");
        }

        bool emailTaken = await _context.Users.AnyAsync(
            u => u.Email == request.Email &&
                u.Id != userId
        );

        if (emailTaken)
        {
            return Result<UserProfileResponse>
                .Fail("Email already taken");
        }

        bool loginTaken = await _context.Users.AnyAsync(
            u => u.Login == request.Username &&
                u.Id != userId
        );

        if (loginTaken)
        {
            return Result<UserProfileResponse>
                .Fail("Login already taken");
        }

        if (user.Email != request.Email)
        {
            user.LastEmail = user.Email;
            user.Email = request.Email;
        }

        user.Login = request.Username;
        user.UserDetails.Firstname = request.Firstname;
        user.UserDetails.Lastname = request.Lastname;
        user.UserDetails.JobTitle = request.JobTitle;
        user.UserDetails.Bio = request.Bio;
        user.UserDetails.AvatarUrl = request.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;
        user.UserDetails.VisibilityId = request.VisibilityId;
        user.UserDetails.ProfileGroups.Clear();
        if (request.GroupIds != null)
        {
            foreach (var groupId in request.GroupIds)
            {
                user.UserDetails.ProfileGroups.Add(new ProfileGroup { GroupId = groupId, UserId = userId });
            }
        }

        await _context.SaveChangesAsync();

        // Re-evaluate visibility for the requester (could be admin, owner, or other user)
        var requestingUserId = GetUserId();
        var requestingUserRole = GetRole();
        bool isOwnProfile = requestingUserId == userId;
        bool isAdmin = requestingUserRole == "Admin";
        bool isGroupMember = false;
        var allowedGroupIds = user.UserDetails.ProfileGroups.Select(pg => pg.GroupId).ToList();
        if (user.UserDetails.VisibilityId == 3) // Group visibility
        {
            isGroupMember = await _context.GroupUsers
                .AnyAsync(gu => gu.UserId == requestingUserId && allowedGroupIds.Contains(gu.GroupId));
        }
        bool canViewProfileMeta = isAdmin || isOwnProfile || user.UserDetails.VisibilityId == 1 || (user.UserDetails.VisibilityId == 3 && isGroupMember);

        return Result<UserProfileResponse>
            .Success(
                new UserProfileResponse
                {
                    UserId = user.Id,
                    Login = user.Login,
                    Email = canViewProfileMeta ? user.Email : "",
                    FirstName = user.UserDetails.Firstname,
                    LastName = user.UserDetails.Lastname,
                    JobTitle = canViewProfileMeta ? user.UserDetails.JobTitle : "",
                    IsActive = user.IsActive,
                    Bio = canViewProfileMeta ? user.UserDetails.Bio : "",
                    AvatarUrl = user.UserDetails.AvatarUrl,
                    Role = user.UserRole.Name,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    VisibilityId = user.UserDetails.VisibilityId,
                    GroupIds = allowedGroupIds,
                    IsProfileRestricted = !canViewProfileMeta,
                    RankingPoints = user.RankingPoints
                }
            );
    }

    public async Task<Result> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => u.Id == userId &&
                    u.DeletedAt == null);

        if (user == null)
        {
            return Result
                .Fail("User not found");
        }

        bool currentPasswordValid =
            BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                user.Password);

        if (!currentPasswordValid)
            return Result
                .Fail(
                    "Current password is incorrect"
                );

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return Result
                .Fail(
                    "Passwords do not match"
                );
        }

        bool samePassword = BCrypt.Net.BCrypt.Verify(
            request.NewPassword,
            user.Password
        );

        if (samePassword)
        {
            return Result
                .Fail(
                    "New password must be different"
                );
        }

        user.LastPassword =
            user.Password;

        user.Password =
            BCrypt.Net.BCrypt.HashPassword(
                request.NewPassword
            );

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result.Success();
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
