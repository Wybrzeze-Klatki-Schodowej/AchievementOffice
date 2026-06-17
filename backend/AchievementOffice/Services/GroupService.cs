using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Services
{
    public class GroupService : IGroupService
    {
        private readonly AppDbContext _context;
        private const int _defaultMaxUser = 100;

        public GroupService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> CreateGroupAsync(CreateGroupRequest request, Guid creatorUserId)
        {
            var groupExists = await _context.Groups.AnyAsync(g => g.Name == request.Name);

            if(groupExists)
            {
                return Result<Guid>.Fail("Group with given name already exists");
            }

            var maxUserCount = request.MaxUserCount > 0 ? request.MaxUserCount : _defaultMaxUser;

            var group = new Group
            {
                GroupId = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                MaxUserCount = maxUserCount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var adminRole = new GroupUserRole
            {
                GroupUserRoleId = Guid.NewGuid(),
                GroupId = group.GroupId,
                Name = "Admin",
                IsAdmin = true
            };

            var memberRole = new GroupUserRole
            {
                GroupUserRoleId = Guid.NewGuid(),
                GroupId = group.GroupId,
                Name = "Member",
                IsAdmin = false
            };

            var groupCreatorMembership = new GroupUser
            {
                GroupId = group.GroupId,
                UserId = creatorUserId,
                GroupUserRoleId = adminRole.GroupUserRoleId
            };

            _context.Groups.Add(group);
            _context.GroupUserRoles.AddRange(adminRole, memberRole);
            _context.GroupUsers.Add(groupCreatorMembership);

            await _context.SaveChangesAsync();

            return Result<Guid>.Success(group.GroupId);
        }

        public async Task<Result<List<GroupResponse>>> GetGroupsAsync(Guid userId)
        {
            var groups = await _context.Groups
                .Where(g => g.DeletedAt == null)
                .Select(g => new GroupResponse
                {
                    GroupId = g.GroupId,
                    Name = g.Name,
                    Description = g.Description,
                    MaxUserCount = g.MaxUserCount,
                    AvatarUrl = g.AvatarUrl,
                    CreatedAt = g.CreatedAt
                })
                .ToListAsync();

            return Result<List<GroupResponse>>.Success(groups);
        }

        public async Task<Result> AddUserToGroupAsync(Guid groupId, Guid userToAddId, Guid groupUserRoleId, Guid creatorUserId)
        {
            bool hasPermissionToAdd = await _context.GroupUsers
                .Include(gu => gu.GroupUserRole)
                .AnyAsync(gu => gu.GroupId == groupId && gu.UserId == creatorUserId && gu.GroupUserRole.IsAdmin);

            if (!hasPermissionToAdd)
                return Result.Fail("No permission to add member to this group");

            var groupRole = await _context.GroupUserRoles.FindAsync(groupUserRoleId);
            
            if (groupRole == null || groupRole.GroupId != groupId)
                return Result.Fail("Invalid role");

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null || group.DeletedAt != null)
                return Result.Fail("Group does not exists");

            bool alreadyMember = await _context.GroupUsers.AnyAsync(gu => gu.GroupId == groupId && gu.UserId == userToAddId);

            if (alreadyMember)
                return Result.Fail("User is already in this group");

            var groupMembership = new GroupUser
            {
                GroupId = groupId,
                UserId = userToAddId,
                GroupUserRoleId = groupUserRoleId
            };

            _context.GroupUsers.Add(groupMembership);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<GroupResponse>> GetGroupByIdAsync(Guid groupId)
        {
            var group = await _context.Groups
                .Where(g => g.GroupId == groupId && g.DeletedAt == null)
                .Select(g => new GroupResponse
                {
                    GroupId = g.GroupId,
                    Name = g.Name,
                    Description = g.Description,
                    MaxUserCount = g.MaxUserCount,
                    AvatarUrl = g.AvatarUrl,
                    CreatedAt = g.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (group == null)
            {
                return Result<GroupResponse>.Fail("Group not found.");
            }

            return Result<GroupResponse>.Success(group);
        }

        public async Task<Result<List<GroupMemberResponse>>> GetGroupMembersAsync(Guid groupId)
        {
            var groupExists = await _context.Groups
                .AnyAsync(g => g.GroupId == groupId && g.DeletedAt == null);

            if (!groupExists)
            {
                return Result<List<GroupMemberResponse>>.Fail("Group not found.");
            }

            var members = await _context.GroupUsers
                .Where(gu => gu.GroupId == groupId)
                .Select(gu => new GroupMemberResponse
                {
                    UserId = gu.UserId,
                    Login = gu.User.Login,
                    FirstName = gu.User.UserDetails.Firstname,
                    LastName = gu.User.UserDetails.Lastname,
                    GroupUserRoleId = gu.GroupUserRoleId,
                    RoleName = gu.GroupUserRole.Name,
                    IsAdmin = gu.GroupUserRole.IsAdmin
                })
                .ToListAsync();

            return Result<List<GroupMemberResponse>>.Success(members);
        }

        public async Task<Result<List<GroupRoleResponse>>> GetGroupRolesAsync(Guid groupId)
        {
            var roles = await _context.GroupUserRoles
                .Where(r => r.GroupId == groupId)
                .Select(r => new GroupRoleResponse
                {
                    GroupUserRoleId = r.GroupUserRoleId,
                    Name = r.Name,
                    IsAdmin = r.IsAdmin
                })
                .ToListAsync();

            return Result<List<GroupRoleResponse>>.Success(roles);
        }

        public async Task<Result> RemoveUserFromGroupAsync(
            Guid groupId,
            Guid userIdToRemove,
            Guid currentUserId,
            bool isGlobalAdmin)
        {
            bool isGroupAdmin = await _context.GroupUsers
                .Include(x => x.GroupUserRole)
                .AnyAsync(x =>
                    x.GroupId == groupId &&
                    x.UserId == currentUserId &&
                    x.GroupUserRole.IsAdmin);

            if (!isGlobalAdmin && !isGroupAdmin)
            {
                return Result.Fail("Forbidden");
            }

            var membership = await _context.GroupUsers
                .Include(x => x.GroupUserRole)
                .FirstOrDefaultAsync(x =>
                    x.GroupId == groupId &&
                    x.UserId == userIdToRemove);

            if (membership == null)
            {
                return Result.Fail("User is not a member of this group");
            }

            if (membership.GroupUserRole.IsAdmin)
            {
                var adminCount = await _context.GroupUsers
                    .Include(x => x.GroupUserRole)
                    .CountAsync(x =>
                        x.GroupId == groupId &&
                        x.GroupUserRole.IsAdmin);

                if (adminCount <= 1)
                {
                    return Result.Fail(
                        "Cannot remove the last administrator");
                }
            }

            _context.GroupUsers.Remove(membership);

            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
