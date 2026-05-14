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
                return new LoginResult() { IsSuccesful = false };

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!isPasswordValid)
                return new LoginResult() { IsSuccesful = false };

            var token = _tokenService.GenerateToken(user);

            if (string.IsNullOrEmpty(token))
                return new LoginResult() { IsSuccesful = false };

            return new LoginResult() { IsSuccesful = true, Token = token };
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
    }
}
