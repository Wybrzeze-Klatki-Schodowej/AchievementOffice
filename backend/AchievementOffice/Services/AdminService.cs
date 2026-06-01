using AchievementOffice.Data;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Services;

public class AdminService : IAdminService
{
	private readonly AppDbContext _context;

	public AdminService(AppDbContext context)
	{
		_context = context;
	}

	public async Task<List<AdminUserProfileResponse>> GetAllUsersAsync(bool? isActiveFilter = null)
	{
		var query = _context.Users
			.Include(u => u.UserDetails)
			.Include(u => u.UserRole)
			.Where(u => u.DeletedAt == null);

		if (isActiveFilter.HasValue)
		{
			query = query.Where(u => u.IsActive == isActiveFilter.Value);
		}

		return await query.Select(u => new AdminUserProfileResponse
		{
			UserId = u.Id,
			Login = u.Login,
			Email = u.Email,
			FirstName = u.UserDetails.Firstname,
			LastName = u.UserDetails.Lastname,
			JobTitle = u.UserDetails.JobTitle,
			Role = u.UserRole.Name,
			IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
			UpdatedAt = u.UpdatedAt
		}).ToListAsync();
	}

	public async Task<AdminStatsResponse> GetStatsAsync()
	{
		var total = await _context.Users.CountAsync( u => u.DeletedAt == null );
		var active = await _context.Users.CountAsync( u => u.DeletedAt == null && u.IsActive );

		return new AdminStatsResponse
		{
			TotalUsers = total,
			ActiveUsers = active,
			InactiveUsers = total - active
		};
	}

	public async Task<Result> UpdateUserStatusAsync(Guid userId, bool isActive)
	{
		var user = await _context.Users
			.FirstOrDefaultAsync( u => u.Id == userId && u.DeletedAt == null );

		if (user == null)
			return Result.Fail( "User not found" );

		user.IsActive = isActive;
		user.UpdatedAt = DateTime.UtcNow;
		await _context.SaveChangesAsync();

		return Result.Success();
	}
}