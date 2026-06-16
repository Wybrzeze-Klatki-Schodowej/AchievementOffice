namespace AchievementOffice.Entities;

public class AchievementApprove
{
	public Guid AchievementApproveId { get; set; }
	public Guid AchievementId { get; set; }
	public Guid UserId { get; set; }
	public User User { get; set; } = null!;
	public bool IsApproved { get; set; }
	public DateTime ApprovedAt { get; set; }
	public DateTime? DeletedAt { get; set; }
}	