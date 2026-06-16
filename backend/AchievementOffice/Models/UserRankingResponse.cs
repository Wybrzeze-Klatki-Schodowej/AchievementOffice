namespace AchievementOffice.Models
{
    public class UserRankingResponse
    {
        public Guid UserId { get; set; }
        public required string UserName { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public string? Avatar {  get; set; }
        public decimal RankingPoints { get; set; }
    }
}
