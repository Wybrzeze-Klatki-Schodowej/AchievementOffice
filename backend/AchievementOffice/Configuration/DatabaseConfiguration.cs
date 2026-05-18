using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Configuration
{
    public class DatabaseConfiguration
    {
        public static string SectionName => nameof(DatabaseConfiguration);

        [Required]
        public string Host { get; set; } = string.Empty;

        [Required]
        [Range(5432, 5432)]
        public int Port { get; set; }

        [Required]
        public string Database { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
