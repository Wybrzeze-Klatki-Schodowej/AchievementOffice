namespace AchievementOffice.Models;

public class UserProfileResponse
{
    public Guid UserId { get; set; }
    public string Login { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string JobTitle { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int VisibilityId { get; set; } = 1;
    public List<Guid> GroupIds { get; set; } = new();
    public bool IsProfileRestricted { get; set; } = true;
}