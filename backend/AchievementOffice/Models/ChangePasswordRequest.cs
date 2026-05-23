using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Models
{
    public class ChangePasswordRequest
    {
        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        [MinLength(8,
            ErrorMessage =
            "Password must be at least 8 characters long.")]
        public required string NewPassword { get; set; }

        [Required]
        public required string ConfirmNewPassword { get; set; }
    }
}