namespace AchievementOffice.Entities;

public class RankingConfiguration
{
    public int Id { get; set; } = 1;
    public decimal ShoutoutBase { get; set; }
    public decimal ReactionShoutOutPoints { get; set; }
    public decimal AchievementApprovedBasePoints { get; set; }
    public decimal DisapprovalPoints { get; set; }
}
