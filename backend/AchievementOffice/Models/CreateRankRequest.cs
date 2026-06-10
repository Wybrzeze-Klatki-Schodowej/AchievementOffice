namespace AchievementOffice.Models
{
    public class CreateRankRequest
    {
        public required string Name { get; set; }
        public decimal? Multiplier { get; set; }
    }
}
