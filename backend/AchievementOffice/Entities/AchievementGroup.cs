namespace AchievementOffice.Entities;
public class AchievementGroup
{
    public Guid AchievementId { get; set; }
    public Achievement Achievement { get; set; } = null!;
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
}