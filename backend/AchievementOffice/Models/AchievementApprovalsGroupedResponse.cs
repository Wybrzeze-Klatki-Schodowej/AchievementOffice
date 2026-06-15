namespace AchievementOffice.Models;

public class AchievementApprovalsGroupedDto
{
    public List<AchievementApproveResponseDto> Approved { get; set; } = [];
    public List<AchievementApproveResponseDto> Denied { get; set; } = [];
}