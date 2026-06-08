namespace AchievementOffice.Models;

public class GroupRoleResponse
{
    public Guid GroupUserRoleId { get; set; }
    public required string Name { get; set; }
    public bool IsAdmin { get; set; }
}
