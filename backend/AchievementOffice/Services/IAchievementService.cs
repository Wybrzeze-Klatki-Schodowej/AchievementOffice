using AchievementOffice.Common;
using AchievementOffice.Models;

namespace AchievementOffice.Services;

public interface IAchievementService
{
    Task<Result<AchievementResponse>> CreateAsync(CreateAchievementRequest createDto);

    Task<Result<List<AchievementResponse>>> GetAllAsync();

    Task<Result<AchievementResponse>> GetByIdAsync(Guid id);

    Task<Result<AchievementResponse>> UpdateAsync(Guid id, UpdateAchievementRequest dto);

    Task<Result<bool>> DeleteAsync(Guid id);

    Task<AchievementApproveResponseDto> ApproveAsync(Guid achievementId, Guid userId, CreateAchievementApproveDto dto);

    Task<List<AchievementApproveResponseDto>> GetApprovalsAsync(Guid achievementId);

    Task<AchievementApprovalSummaryDto> GetApprovalSummaryAsync(Guid achievementId);
}