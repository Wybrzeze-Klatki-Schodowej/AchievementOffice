namespace AchievementOffice.Entities
{
    public class Group
    {
        public Guid GroupId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int MaxUserCount { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();
    }
}
