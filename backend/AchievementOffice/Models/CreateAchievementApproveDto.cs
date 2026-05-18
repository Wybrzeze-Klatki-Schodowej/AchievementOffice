namespace AchievementOffice.Features.Achievements.DTOs;

public class CreateAchievementApproveDto
{
	public Guid AchievementId { get; set; }
	public bool IsApproved { get; set; }
}