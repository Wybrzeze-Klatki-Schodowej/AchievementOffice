namespace AchievementOffice.Entities;

public class AchievementVerificationRequest
{
    public Guid Id { get; set; }

    public Guid AchievementId { get; set; }
    public Achievement Achievement { get; set; } = null!;

    public Guid RequesterUserId { get; set; }
    public User RequesterUser { get; set; } = null!;

    public Guid TargetUserId { get; set; }
    public User TargetUser { get; set; } = null!;

    public VerificationRequestStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}

public enum VerificationRequestStatus
{
    Pending = 0,
    Approved = 1,
    Denied = 2
}