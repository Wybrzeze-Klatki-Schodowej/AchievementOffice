namespace AchievementOffice.Models;

public class GroupMemberResponse
{
    public Guid UserId { get; set; }
    public required string Login { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Guid GroupUserRoleId { get; set; }
    public required string RoleName { get; set; }
    public bool IsAdmin { get; set; }
}
