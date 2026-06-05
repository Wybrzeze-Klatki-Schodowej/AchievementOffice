namespace AchievementOffice.Entities
{
    public class GroupUser
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid GroupId { get; set; }
        public Group Group { get; set; } = null!;
        public int GroupUserRoleId { get; set; }
        public GroupUserRole GroupUserRole { get; set; } = null!;
    }
}
