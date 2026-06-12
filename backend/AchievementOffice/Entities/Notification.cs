namespace AchievementOffice.Entities;

public class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public NotificationType Type { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public Guid? AchievementVerificationRequestId { get; set; }

    public AchievementVerificationRequest? AchievementVerificationRequest { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}

public enum NotificationType
{
    AchievementVerificationRequest = 1,
    System = 2
}