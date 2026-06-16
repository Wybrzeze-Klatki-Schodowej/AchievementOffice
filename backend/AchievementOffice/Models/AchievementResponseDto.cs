namespace AchievementOffice.Models;

public class AchievementResponse
{
    public Guid AchievementId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int VisibilityId { get; set; }
    public List<Guid> GroupIds { get; set; } = new();
}