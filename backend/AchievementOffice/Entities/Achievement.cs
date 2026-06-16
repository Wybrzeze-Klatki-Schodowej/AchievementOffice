using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Entities;

public class Achievement
{
    public Guid AchievementId { get; set; }

    public Guid UserId { get; set; }

    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int VisibilityId { get; set; } = (int)VisibilityMode.Public;
    public Visibility Visibility { get; set; } = null!;
    public ICollection<AchievementGroup> AchievementGroups { get; set; } = new List<AchievementGroup>();
}