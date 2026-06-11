namespace AchievementOffice.Models;

public class GroupResponse
{
    public Guid GroupId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int MaxUserCount { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
