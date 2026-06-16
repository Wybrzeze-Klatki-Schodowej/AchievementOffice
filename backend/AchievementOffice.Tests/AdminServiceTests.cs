using Xunit;
using Microsoft.EntityFrameworkCore;
using AchievementOffice.Data;
using AchievementOffice.Services;
using AchievementOffice.Models;
using AchievementOffice.Entities;

namespace AchievementOffice.Tests
{
    public class AdminServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase( databaseName: Guid.NewGuid().ToString() )
                .Options;
            return new AppDbContext( options );
        }

        private User MakeUser(bool isActive = true, DateTime? deletedAt = null) => new User
        {
            Id = Guid.NewGuid(),
            Login = Guid.NewGuid().ToString(),
            Email = $"{Guid.NewGuid()}@test.com",
            Password = "hash",
            IsActive = isActive,
            DeletedAt = deletedAt
        };

        [Fact]
        public async Task GetAllUsersAsync_NoFilter_ReturnsAllNonDeletedUsers()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            var role = new UserRole { Id = Guid.NewGuid(), Name = "User" };
            var activeUser = MakeUser( isActive: true );
            activeUser.UserRole = role;
            activeUser.UserDetails = new UserDetails { Firstname = "John", Lastname = "Doe", JobTitle = "Dev" };

            var inactiveUser = MakeUser( isActive: false );
            inactiveUser.UserRole = role;
            inactiveUser.UserDetails = new UserDetails { Firstname = "Jane", Lastname = "Doe", JobTitle = "Dev" };

            var deletedUser = MakeUser( isActive: true, deletedAt: DateTime.UtcNow );
            deletedUser.UserRole = role;
            deletedUser.UserDetails = new UserDetails { Firstname = "Ghost", Lastname = "User", JobTitle = "Dev" };

            context.Users.AddRange( activeUser, inactiveUser, deletedUser );
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllUsersAsync();

            // Assert
            Assert.Equal( 2, result.Count );
            Assert.Contains( result, u => u.Login == activeUser.Login );
            Assert.Contains( result, u => u.Login == inactiveUser.Login );
            Assert.DoesNotContain( result, u => u.Login == deletedUser.Login );
        }

        [Fact]
        public async Task GetAllUsersAsync_WithActiveFilter_ReturnsOnlyActiveUsers()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            var role = new UserRole { Id = Guid.NewGuid(), Name = "User" };
            var activeUser = MakeUser( isActive: true );
            activeUser.UserRole = role;
            activeUser.UserDetails = new UserDetails { Firstname = "A", Lastname = "B", JobTitle = "Dev" };

            var inactiveUser = MakeUser( isActive: false );
            inactiveUser.UserRole = role;
            inactiveUser.UserDetails = new UserDetails { Firstname = "C", Lastname = "D", JobTitle = "Dev" };

            context.Users.AddRange( activeUser, inactiveUser );
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllUsersAsync( isActiveFilter: true );

            // Assert
            Assert.Single( result );
            Assert.Equal( activeUser.Login, result[0].Login );
        }

        [Fact]
        public async Task GetStatsAsync_ReturnsCorrectCounts()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            context.Users.AddRange(
                MakeUser( isActive: true ),
                MakeUser( isActive: true ),
                MakeUser( isActive: false ),
                MakeUser( isActive: true, deletedAt: DateTime.UtcNow )
            );
            await context.SaveChangesAsync();

            // Act
            var stats = await service.GetStatsAsync();

            // Assert
            Assert.Equal( 3, stats.TotalUsers );
            Assert.Equal( 2, stats.ActiveUsers );
            Assert.Equal( 1, stats.InactiveUsers );
        }

        [Fact]
        public async Task UpdateUserStatusAsync_ExistingUser_UpdatesStatusAndReturnsSuccess()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            var user = MakeUser( isActive: false );
            context.Users.Add( user );
            await context.SaveChangesAsync();

            // Act
            var result = await service.UpdateUserStatusAsync( user.Id, true );

            // Assert
            Assert.True( result.IsSuccess );
            var updated = await context.Users.FindAsync( user.Id );
            Assert.True( updated!.IsActive );
        }

        [Fact]
        public async Task UpdateUserStatusAsync_NonExistingUser_ReturnsFail()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            // Act
            var result = await service.UpdateUserStatusAsync( Guid.NewGuid(), true );

            // Assert
            Assert.False( result.IsSuccess );
            Assert.Equal( "User not found", result.ErrorMessage );
        }

        [Fact]
        public async Task GetRanksAsync_ReturnsAllRanks()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            context.Ranks.AddRange(
                new Rank { Id = Guid.NewGuid(), Name = "Bronze", Multiplier = 1.0m },
                new Rank { Id = Guid.NewGuid(), Name = "Silver", Multiplier = 1.5m }
            );
            await context.SaveChangesAsync();

            // Act
            var results = await service.GetRanksAsync();

            // Assert
            Assert.Equal( 2, results.Count );
            Assert.Contains( results, r => r.Name == "Bronze" );
            Assert.Contains( results, r => r.Name == "Silver" );
        }

        [Fact]
        public async Task UpdateUserRankAsync_ValidUserAndRank_UpdatesRankAndReturnsSuccess()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            var user = MakeUser();
            var rank = new Rank { Id = Guid.NewGuid(), Name = "Gold", Multiplier = 2.0m };
            context.Users.Add( user );
            context.Ranks.Add( rank );
            await context.SaveChangesAsync();

            // Act
            var result = await service.UpdateUserRankAsync( user.Id, rank.Id );

            // Assert
            Assert.True( result.IsSuccess );
            var updated = await context.Users.FindAsync( user.Id );
            Assert.Equal( rank.Id, updated!.RankId );
        }

        [Fact]
        public async Task UpdateUserRankAsync_NullRankId_ClearsUserRankAndReturnsSuccess()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            var user = MakeUser();
            user.RankId = Guid.NewGuid();
            context.Users.Add( user );
            await context.SaveChangesAsync();

            // Act
            var result = await service.UpdateUserRankAsync( user.Id, null );

            // Assert
            Assert.True( result.IsSuccess );
            var updated = await context.Users.FindAsync( user.Id );
            Assert.Null( updated!.RankId );
        }

        [Fact]
        public async Task UpdateUserRankAsync_InvalidRank_ReturnsFail()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            var user = MakeUser();
            context.Users.Add( user );
            await context.SaveChangesAsync();

            // Act
            var result = await service.UpdateUserRankAsync( user.Id, Guid.NewGuid() );

            // Assert
            Assert.False( result.IsSuccess );
        }

        [Fact]
        public async Task DeleteCommentAsync_ExistingComment_SetsDeletedAtAndReturnsSuccess()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            var author = MakeUser();
            var profileUser = MakeUser();
            context.Users.AddRange( author, profileUser );
            await context.SaveChangesAsync();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Test",
                AuthorId = author.Id,
                ProfileUserId = profileUser.Id
            };
            context.Comments.Add( comment );
            await context.SaveChangesAsync();

            // Act
            var result = await service.DeleteCommentAsync( comment.Id );

            // Assert
            Assert.True( result.IsSuccess );
            var deleted = await context.Comments.FindAsync( comment.Id );
            Assert.NotNull( deleted!.DeletedAt );
        }

        [Fact]
        public async Task DeleteAchievementAsync_ExistingAchievement_SetsDeletedAtAndReturnsSuccess()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AdminService( context );

            var achievement = new Achievement
            {
                AchievementId = Guid.NewGuid(),
                Title = "Test",
                UserId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Achievements.Add( achievement );
            await context.SaveChangesAsync();

            // Act
            var result = await service.DeleteAchievementAsync( achievement.AchievementId );

            // Assert
            Assert.True( result.IsSuccess );
            var deleted = await context.Achievements.FindAsync( achievement.AchievementId );
            Assert.NotNull( deleted!.DeletedAt );
        }
    }
}