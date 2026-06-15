namespace AchievementOffice.Models
{
    public class GroupRankingResponse
    {
        public Guid GroupId { get; set; }
        public required string GroupName { get; set; }
        public string? Avatar { get; set; }
        public decimal RankingPoints { get; set; }
    }
}
