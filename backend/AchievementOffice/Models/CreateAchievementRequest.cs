namespace AchievementOffice.Models;

public class CreateAchievementRequest
{
    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
}