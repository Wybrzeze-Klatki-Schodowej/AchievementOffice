namespace AchievementOffice.Entities;

public class UserDetails
{
    public Guid UserId {  get; set; }
    public User User { get; set; } = null!;
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public required string JobTitle { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }

    public int VisibilityId { get; set; } = (int)VisibilityMode.Public;
    public Visibility Visibility { get; set; } = null!;

    public ICollection<ProfileGroup> ProfileGroups { get; set; } = new List<ProfileGroup>();
}