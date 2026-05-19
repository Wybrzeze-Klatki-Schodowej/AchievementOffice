using AchievementOffice.Data;
using AchievementOffice.Features.Achievements;
using AchievementOffice.Features.Achievements.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Tests;

public class AchievementServiceTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString() )
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task ApproveAsync_FirstVote_ReturnsDto() // czy pierwszy glos dziala poprawnie
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new AchievementService( context );
        var UserId = Guid.NewGuid();
        var dto = new CreateAchievementApproveDto
        {
            AchievementId = Guid.NewGuid(),
            IsApproved = true
        };

        var result = await service.ApproveAsync(UserId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.AchievementId, result.AchievementId);
        Assert.Equal(UserId, result.UserId);
        Assert.True(result.IsApproved);
    }

    [Fact]
    public async Task ApproveAsync_SecondVote_ThrowsException() // czy drugi glos tego samego uzytkownika na ten sam achievement rzuca wyjatkiem
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new AchievementService( context );
        var UserId = Guid.NewGuid();
        var dto = new CreateAchievementApproveDto
        {
            AchievementId = Guid.NewGuid(),
            IsApproved = true
        };
        await service.ApproveAsync(UserId, dto);
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>( () => service.ApproveAsync(UserId, dto) );
    }

    [Fact]
    public async Task ApproveAsync_DifferentUsers_Success() // czy rozne glosy roznych uzytkownikow na ten sam achievement dzialaja poprawnie
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new AchievementService( context );
        var AchievementId = Guid.NewGuid();
        var dto1 = new CreateAchievementApproveDto
        {
            AchievementId = AchievementId,
            IsApproved = true
        };
        var dto2 = new CreateAchievementApproveDto
        {
            AchievementId = AchievementId,
            IsApproved = false
        };
        var UserId1 = Guid.NewGuid();
        var UserId2 = Guid.NewGuid();
        // Act
        var result1 = await service.ApproveAsync(UserId1, dto1);
        var result2 = await service.ApproveAsync(UserId2, dto2);
        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(dto1.AchievementId, result1.AchievementId);
        Assert.Equal(dto2.AchievementId, result2.AchievementId);
        Assert.Equal(UserId1, result1.UserId);
        Assert.Equal(UserId2, result2.UserId);
        Assert.True(result1.IsApproved);
        Assert.False(result2.IsApproved);
    }

   

    [Fact]
    public async Task GetApprovalSummaryAsync_NoVotes_ReturnsZeros() // czy podsumowanie glosowania na achievement bez glosow zwraca zera
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new AchievementService( context );
        var AchievementId = Guid.NewGuid();
        // Act
        var summary = await service.GetApprovalSummaryAsync(AchievementId);
        // Assert
        Assert.NotNull(summary);
        Assert.Equal(0, summary.Approved);
        Assert.Equal(0, summary.Denied);
    }

    [Fact]
    public async Task GetApprovalSummaryAsync_WithVotes_ReturnsCorrectCounts()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new AchievementService( context );
        var achievementId = Guid.NewGuid();

        await service.ApproveAsync( Guid.NewGuid(), new CreateAchievementApproveDto
        { AchievementId = achievementId, IsApproved = true } );
        await service.ApproveAsync( Guid.NewGuid(), new CreateAchievementApproveDto
        { AchievementId = achievementId, IsApproved = true } );
        await service.ApproveAsync( Guid.NewGuid(), new CreateAchievementApproveDto
        { AchievementId = achievementId, IsApproved = false } );

        // Act
        var result = await service.GetApprovalSummaryAsync( achievementId );

        // Assert
        Assert.Equal( 2, result.Approved );
        Assert.Equal( 1, result.Denied );
    }

    // ACHIEVEMENT CRUD

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsAchievement()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new AchievementService( context );
        var dto = new CreateAchievementDto
        {
            UserId = Guid.NewGuid(),
            Title = "Test Achievement",
            Description = "Test Description"
        };

        // Act
        var result = await service.CreateAsync( dto );

        // Assert
        Assert.NotNull( result );
        Assert.Equal( dto.Title, result.Title );
        Assert.Equal( dto.UserId, result.UserId );
    }

    [Fact]
    public async Task DeleteAsync_ExistingAchievement_ReturnsTrueAndSetsDeletedAt()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new AchievementService( context );
        var created = await service.CreateAsync( new CreateAchievementDto
        {
            UserId = Guid.NewGuid(),
            Title = "To delete"
        } );

        // Act
        var result = await service.DeleteAsync( created.AchievementId );

        // Assert
        Assert.True( result );
        var deleted = await context.Achievements
            .FirstOrDefaultAsync( a => a.AchievementId == created.AchievementId );
        Assert.NotNull( deleted!.DeletedAt );
    }

    [Fact]
    public async Task DeleteAsync_NonExistingAchievement_ReturnsFalse()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new AchievementService( context );

        // Act
        var result = await service.DeleteAsync( Guid.NewGuid() );

        // Assert
        Assert.False( result );
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyNonDeleted()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new AchievementService( context );

        var created = await service.CreateAsync( new CreateAchievementDto
        {
            UserId = Guid.NewGuid(),
            Title = "Active"
        } );
        await service.CreateAsync( new CreateAchievementDto
        {
            UserId = Guid.NewGuid(),
            Title = "To delete"
        } );
        await service.DeleteAsync( created.AchievementId );

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Single( result );
        Assert.Equal( "To delete", result[0].Title );
    }
}
