namespace AchievementOffice.Models;

public class CreateAchievementRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int VisibilityId { get; set; } = 1;
    public List<Guid>? GroupIds { get; set; }
}