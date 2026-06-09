using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result>
        CreateAchievementVerificationRequestNotificationAsync(
            Guid userId,
            Guid verificationRequestId,
            string achievementTitle)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.AchievementVerificationRequest,
            Title = "Achievement verification request",
            Message = $"You have been asked to verify achievement '{achievementTitle}'.",
            AchievementVerificationRequestId = verificationRequestId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<List<NotificationResponse>>>
        GetUserNotificationsAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Include(n => n.AchievementVerificationRequest!)
                .ThenInclude(r => r.Achievement)
            .Include(n => n.AchievementVerificationRequest!)
                .ThenInclude(r => r.RequesterUser)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        var response = notifications
            .Select(MapToResponse)
            .ToList();

        return Result<List<NotificationResponse>>
            .Success(response);
    }

    public async Task<Result> DeleteAsync(Guid notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(
                n => n.Id == notificationId);

        if (notification == null)
        {
            return Result.Fail("Notification not found.");
        }

        _context.Notifications.Remove(notification);

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    private static NotificationResponse
        MapToResponse(Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            VerificationRequestId = notification.AchievementVerificationRequestId,
            AchievementId = notification.AchievementVerificationRequest?.AchievementId,
            AchievementOwnerId = notification.AchievementVerificationRequest?.Achievement?.UserId,
            AchievementTitle = notification.AchievementVerificationRequest?.Achievement?.Title,
            AchievementDescription = 
                notification.AchievementVerificationRequest?.Achievement?.Description,
            AchievementOwnerLogin = 
                notification.AchievementVerificationRequest?.RequesterUser?.Login
        };
    }
}