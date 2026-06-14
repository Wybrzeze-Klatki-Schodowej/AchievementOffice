namespace AchievementOffice.Entities;

public class Visibility
{
    public int VisibilityModeId { get; set; }
    public required string VisibilityModeName { get; set; }

    public ICollection<Shoutout> Shoutouts { get; set; } = new List<Shoutout>();
    public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
    public ICollection<UserDetails> UserDetailsList { get; set; } = new List<UserDetails>();
}