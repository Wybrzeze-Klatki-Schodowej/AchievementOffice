namespace AchievementOffice.Models
{
    public class RankResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Multiplier { get; set; }
    }
}
