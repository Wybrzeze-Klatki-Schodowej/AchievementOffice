namespace AchievementOffice.Models;

public class UpdateAchievementRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
}