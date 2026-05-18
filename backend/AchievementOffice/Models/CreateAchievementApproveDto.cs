namespace AchievementOffice.Models;

public class CreateAchievementApproveDto
{
	public Guid AchievementId { get; set; }
	public bool IsApproved { get; set; }
}