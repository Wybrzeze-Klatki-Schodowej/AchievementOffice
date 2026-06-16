namespace AchievementOffice.Models;

public class AdminUserProfileResponse
{
    public Guid UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public decimal RankingPoints { get; set; }
    public Guid? RankId { get; set; }
    public string? RankName { get; set; }
}