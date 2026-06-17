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
			.Include(u => u.Rank)
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
			RankId = u.RankId,
			RankName = u.Rank != null ? u.Rank.Name : null,
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

    public async Task<List<RankResponse>> GetRanksAsync()
    {
        return await _context.Ranks
            .Select(r => new RankResponse
            {
                Id = r.Id,
                Name = r.Name,
                Multiplier = r.Multiplier
            }).ToListAsync();
    }

    public async Task<Result> UpdateUserRankAsync(Guid userId, Guid? rankId) {
		var user = await _context.Users
			.FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);

		if (user == null)
            return Result.Fail("User not found");

        if (rankId.HasValue)
        {
            var rankExists = await _context.Ranks.AnyAsync(r => r.Id == rankId.Value);
            if (!rankExists)
                return Result.Fail("Rank not found");
        }

        user.RankId = rankId;
		user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

		return Result.Success();
    }

    public async Task<Result> DeleteCommentAsync(Guid commentId)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId && c.DeletedAt == null);

        if (comment == null)
            return Result.Fail("Comment not found");

        comment.DeletedAt = DateTime.UtcNow;
        comment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAchievementAsync(Guid achievementId)
    {
        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == achievementId && a.DeletedAt == null);
        if (achievement == null)
            return Result.Fail("Achievement not found");
        achievement.DeletedAt = DateTime.UtcNow;
        achievement.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> CreateRankAsync(CreateRankRequest request)
    {
        var rank = new Entities.Rank
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Multiplier = request.Multiplier ?? 1.0m
        };
        _context.Ranks.Add(rank);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

	public async Task<Result> DeleteShoutoutAsync(Guid shoutoutId)
    {
        var shoutout = await _context.Shoutouts
            .FirstOrDefaultAsync(s => s.ShoutoutId == shoutoutId && s.DeletedAt == null);
        if (shoutout == null)
            return Result.Fail("Shoutout not found");
        shoutout.DeletedAt = DateTime.UtcNow;
        shoutout.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Result.Success();
    }
}