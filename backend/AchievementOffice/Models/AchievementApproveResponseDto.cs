namespace AchievementOffice.Models;

public class AchievementApproveResponseDto
{
    public Guid AchievementApproveId { get; set; }
    public Guid AchievementId { get; set; }
    public Guid UserId { get; set; }
    public Guid OwnerId { get; set; }
    public string? UserLogin { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public bool? IsApproved { get; set; }
    public bool? PrevApproved { get; set; }
    public DateTime ApprovedAt { get; set; }
}