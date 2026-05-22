namespace AchievementOffice.Entities;

public class AchievementApprove
{
	public Guid AchievementApproveId { get; set; }
	public Guid AchievementId { get; set; }
	public Guid UserId { get; set; }
	public bool IsApproved { get; set; }
	public DateTime ApprovedAt { get; set; }
	public DateTime? DeletedAt { get; set; }
}	