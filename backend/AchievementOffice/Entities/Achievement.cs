using System.ComponentModel.DataAnnotations;

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
}