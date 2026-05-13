using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Models
{
    public class LoginRequest
    {
        public required string Login { get; set; }

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public required string Password { get; set; }
    }
}
