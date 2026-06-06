namespace AchievementOffice.Entities
{
    public class GroupUserRole
    {
        public Guid GroupUserRoleId { get; set; }
        public required string Name { get; set; }
        public bool IsAdmin { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; } = null!;
    }
}
