using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public UserService(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.Include(u => u.UserRole).Include(u => u.UserDetails).FirstOrDefaultAsync(u => u.Login == request.Login);

            if (user == null)
                return new LoginResult() { IsSuccessful = false };

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!isPasswordValid)
                return new LoginResult() { IsSuccessful = false };

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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserRegistrationResult() { IsSuccessful = true };
        }

        public async Task<UserProfileResponse?> GetUserProfileAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.UserDetails)
                .Include(u => u.UserRole)
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileResponse
                {
                    UserId = u.Id,
                    Login = u.Login,
                    Email = u.Email,
                    FirstName = u.UserDetails.Firstname,
                    LastName = u.UserDetails.Lastname,
                    JobTitle = u.UserDetails.JobTitle,
                    Bio = u.UserDetails.Bio,
                    AvatarUrl = u.UserDetails.AvatarUrl,
                    Role = u.UserRole.Name,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync();
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
                    Bio = u.UserDetails.Bio,
                    AvatarUrl = u.UserDetails.AvatarUrl,
                    Role = u.UserRole.Name,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return users;
        }

        public async Task<UserProfileResponse?> UpdateUserAsync(
            Guid userId,
            UpdateUserRequest request)
        {
            var user = await _context.Users
                .Include(u => u.UserDetails)
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(
                    u => u.Id == userId &&
                        u.DeletedAt == null
                );

            if (user == null)
                return null;

            bool emailTaken = await _context.Users.AnyAsync(
                u => u.Email == request.Email &&
                    u.Id != userId
            );

            if (emailTaken)
            {
                throw new Exception("Email already taken");
            }

            bool loginTaken = await _context.Users.AnyAsync(
                u => u.Login == request.Username &&
                    u.Id != userId
            );

            if (loginTaken)
            {
                throw new Exception("Login already taken");
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

            await _context.SaveChangesAsync();

            return new UserProfileResponse
            {
                UserId = user.Id,
                Login = user.Login,
                Email = user.Email,
                FirstName = user.UserDetails.Firstname,
                LastName = user.UserDetails.Lastname,
                JobTitle = user.UserDetails.JobTitle,
                Bio = user.UserDetails.Bio,
                AvatarUrl = user.UserDetails.AvatarUrl,
                Role = user.UserRole.Name,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<bool> ChangePasswordAsync(
            Guid userId,
            ChangePasswordRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == userId &&
                        u.DeletedAt == null);

            if (user == null)
                return false;

            bool currentPasswordValid =
                BCrypt.Net.BCrypt.Verify(
                    request.CurrentPassword,
                    user.Password);

            if (!currentPasswordValid)
                throw new Exception(
                    "Current password is incorrect"
                );

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                throw new Exception("Passwords do not match");
            }

            bool samePassword = BCrypt.Net.BCrypt.Verify(
                request.NewPassword,
                user.Password
            );

            if (samePassword)
            {
                throw new Exception(
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

            return true;
        }
    }
}
