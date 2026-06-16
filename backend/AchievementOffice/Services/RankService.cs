using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Services
{
    public class RankService : IRankService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RankService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private static RankResponse MapToDto(Rank rank)
        {
            return new RankResponse
            {
                Id = rank.Id,
                Name = rank.Name,
                Multiplier = rank.Multiplier
            };
        }

        private static UserResponse MapToDto(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Login = user.Login
            };
        }

        public async Task<Result<RankResponse>> CreateRank(CreateRankRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return Result<RankResponse>.Fail("Name of rank is required");

            var rank = new Rank
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Multiplier = dto.Multiplier ?? 1m
            };

            _context.Ranks.Add(rank);
            await _context.SaveChangesAsync();

            return Result<RankResponse>.Success(MapToDto(rank));
        }

        public async Task<Result> DeleteRank(Guid rankId)
        {
            var rank = await _context.Ranks.FindAsync(rankId);
            if (rank == null) return Result.Fail("Rank has not been found");

            var isAssigned = await _context.Users.AnyAsync(u => u.RankId == rankId);
            if (isAssigned) return Result.Fail("Cannot delete rank because it is assigned to users");

            _context.Ranks.Remove(rank);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<List<RankResponse>>> GetAllRanks()
        {
            var ranks = await _context.Ranks.ToListAsync();

            var res = ranks.Select(MapToDto).ToList();
            return Result<List<RankResponse>>.Success(res);
        }

        public async Task<Result<List<UserResponse>>> GetAllUsersWithRank(Guid rankId)
        {
            var rankExist = await _context.Ranks.AnyAsync(r => r.Id == rankId);

            if (!rankExist) return Result<List<UserResponse>>.Fail("Rank does not exist");

            var users = await _context.Users.Where(u => u.RankId == rankId).ToListAsync();
            var res = users.Select(MapToDto).ToList();
            return Result<List<UserResponse>>.Success(res);
        }

        public async Task<Result<RankResponse>> GetRank(Guid rankId)
        {
            var rank = await _context.Ranks.FindAsync(rankId);
            if (rank == null) return Result<RankResponse>.Fail("Rank has not been found");
            return Result<RankResponse>.Success(MapToDto(rank));
        }

        public async Task<Result<RankResponse>> UpdateRank(Guid rankId, RankRequest dto)
        {
            var prevRank = await _context.Ranks.FindAsync(rankId);
            if (prevRank == null) return Result<RankResponse>.Fail("Rank has not been found");

            if (dto.Name != null && dto.Name.Trim().Length == 0) return Result<RankResponse>.Fail("Rank requires name");

            prevRank.Name = dto.Name ?? prevRank.Name;
            prevRank.Multiplier = dto.Multiplier ?? prevRank.Multiplier;

            await _context.SaveChangesAsync();
            return Result<RankResponse>.Success(MapToDto(prevRank));
        }
    }
}
