using AchievementOffice.Entities;

namespace AchievementOffice.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}