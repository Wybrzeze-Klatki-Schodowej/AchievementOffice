using AchievementOffice.Models;

namespace AchievementOffice.Services
{
    public interface IGroupService
    {
        Task<Result<Guid>> CreateGroupAsync(CreateGroupRequest request, Guid creatorUserId);
        Task<Result<List<GroupResponse>>> GetGroupsAsync(Guid userId);
        Task<Result> AddUserToGroupAsync(Guid groupId, Guid userToAddId, Guid groupUserRoleId, Guid creatorUserId);
        Task<Result<GroupResponse>> GetGroupByIdAsync(Guid groupId);
        Task<Result<List<GroupMemberResponse>>> GetGroupMembersAsync(Guid groupId);
        Task<Result<List<GroupRoleResponse>>> GetGroupRolesAsync(Guid groupId);
    }
}
