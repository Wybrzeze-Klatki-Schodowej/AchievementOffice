using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Configuration;

public class CorsConfiguration
{
    public static string SectionName => nameof(CorsConfiguration);

    [Required]
    [Url]
    public string FrontUrl { get; set; } = string.Empty;
}