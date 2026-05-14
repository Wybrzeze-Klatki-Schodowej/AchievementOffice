using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Configuration
{
    public class JwtConfiguration
    {
        public static string SectionName => nameof(JwtConfiguration);

        [Required]
        [Url]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        public string Audience { get; set; } = string.Empty;

        [Required]
        public string SigningKey { get; set; } = string.Empty;
    }
}
