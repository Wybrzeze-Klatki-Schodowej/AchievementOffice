using AchievementOffice.Entities;
using AchievementOffice.Models;

namespace AchievementOffice.Services;

public interface INotificationService
{
    Task<Result> CreateAchievementVerificationRequestNotificationAsync(
        Guid userId,
        Guid verificationRequestId,
        string achievementTitle);

    Task<Result<List<NotificationResponse>>> GetUserNotificationsAsync(Guid userId);

    Task<Result> DeleteAsync(Guid notificationId);
}