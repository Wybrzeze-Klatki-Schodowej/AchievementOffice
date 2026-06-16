namespace AchievementOffice.Entities;

public class ShoutoutGroup
{
    public Guid ShoutoutId { get; set; }
    public Shoutout Shoutout { get; set; } = null!;

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
}