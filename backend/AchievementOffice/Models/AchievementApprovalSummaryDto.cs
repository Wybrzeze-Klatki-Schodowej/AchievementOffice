namespace AchievementOffice.Models;

public class AchievementApprovalSummaryDto
{
    public int Approved { get; set; }
    public int Denied { get; set; }

    public bool? CurrentUserVote { get; set; }
}