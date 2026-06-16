namespace AchievementOffice.Entities
{
    public class Rank
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public decimal Multiplier { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
