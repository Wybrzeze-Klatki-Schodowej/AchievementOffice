namespace AchievementOffice.Entities
{
    public class UserDetails
    {
        public Guid UserId {  get; set; }
        public User User { get; set; } = null!;
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string JobTitle { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
