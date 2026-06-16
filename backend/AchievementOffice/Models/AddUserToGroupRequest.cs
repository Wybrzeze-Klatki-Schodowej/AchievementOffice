using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Models;

public class AddUserToGroupRequest
{
    [Required(ErrorMessage = "The User ID is required.")]
    public Guid? UserId { get; set; }

    [Required(ErrorMessage = "The Role ID is required.")]
    public Guid? RoleId { get; set; }
}
