using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Models;

public class CreateGroupRequest
{
    [Required(ErrorMessage = "Group name cannot be empty")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Group description cannot be empty")]
    public required string Description { get; set; }
    public int MaxUserCount { get; set; }
}
