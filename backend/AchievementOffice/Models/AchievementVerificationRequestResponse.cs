namespace AchievementOffice.Models;

public class AchievementVerificationRequestResponse
{
    public Guid Id { get; set; }

    public Guid AchievementId { get; set; }

    public Guid RequesterUserId { get; set; }

    public string RequesterLogin { get; set; } = string.Empty;

    public Guid TargetUserId { get; set; }

    public string TargetUserLogin { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? RespondedAt { get; set; }
}