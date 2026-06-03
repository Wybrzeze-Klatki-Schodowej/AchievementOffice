using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Models
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        [RegularExpression(RegexPatterns.Email, ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [RegularExpression(@"^\S+$", ErrorMessage = "Username cannot contain spaces")]
        [Required(ErrorMessage = "Username cannot be empty")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "First name cannot be empty")]
        public required string Firstname { get; set; }

        [Required(ErrorMessage = "Last name cannot be empty")]
        public required string Lastname { get; set; }

        [Required(ErrorMessage = "Job title cannot be empty")]
        public required string JobTitle { get; set; }

        public string? Bio { get; set; }

        public string? AvatarUrl { get; set; }
    }
}