namespace AchievementOffice.Features.Achievements.DTOs;

public class AchievementResponseDto
{
    public Guid AchievementId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}