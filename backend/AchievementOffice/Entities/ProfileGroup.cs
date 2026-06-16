namespace AchievementOffice.Entities;

public class ProfileGroup
{
    public Guid UserId { get; set; }
    public UserDetails UserDetails { get; set; } = null!;

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
}