using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Models
{
    public class UpdateUserRequest
    {
        public required string Email { get; set; }

        public required string Username { get; set; }

        public required string Firstname { get; set; }

        public required string Lastname { get; set; }

        public required string JobTitle { get; set; }

        public string? Bio { get; set; }

        public string? AvatarUrl { get; set; }
    }
}