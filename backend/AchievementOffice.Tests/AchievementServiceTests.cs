using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AchievementOffice.Data;
using AchievementOffice.Services;
using AchievementOffice.Models;

namespace AchievementOffice.Tests;

public class AchievementServiceTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private Mock<IHttpContextAccessor> CreateMockHttpContextAccessor()
    {
        return new Mock<IHttpContextAccessor>();
    }

    [Fact]
    public async Task ApproveAsync_FirstVote_ReturnsDtoWithCorrectStatus()
    {
        // Arrange
        var mockHttpContextAccessor = CreateMockHttpContextAccessor();
        var context = CreateInMemoryContext();
        var service = new AchievementService(context, mockHttpContextAccessor.Object);

        var userId = Guid.NewGuid();
        var achievementId = Guid.NewGuid();
        var dto = new CreateAchievementApproveDto
        {
            IsApproved = true
        };

        // Act
        var result = await service.ApproveAsync(achievementId, userId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(achievementId, result.AchievementId);
        Assert.Equal(userId, result.UserId);
        Assert.True(result.IsApproved);
    }

    [Fact]
    public async Task ApproveAsync_SecondSameVote_PerformsSoftDeleteAndReturnsNullApproved()
    {
        // Arrange
        var mockHttpContextAccessor = CreateMockHttpContextAccessor();
        var context = CreateInMemoryContext();
        var service = new AchievementService(context, mockHttpContextAccessor.Object);

        var userId = Guid.NewGuid();
        var achievementId = Guid.NewGuid();
        var dto = new CreateAchievementApproveDto
        {
            IsApproved = true
        };

        // Pierwsze klikniêcie (dodanie g³osu)
        await service.ApproveAsync(achievementId, userId, dto);

        // Act - Drugie klikniêcie (odklikniêcie/cofniêcie g³osu)
        var result = await service.ApproveAsync(achievementId, userId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.IsApproved);

        var dbRecord = await context.AchievementApproves.IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.AchievementId == achievementId && a.UserId == userId);
        Assert.NotNull(dbRecord!.DeletedAt);
    }

    [Fact]
    public async Task ApproveAsync_DifferentUsers_Success()
    {
        // Arrange
        var mockHttpContextAccessor = CreateMockHttpContextAccessor();
        var context = CreateInMemoryContext();
        var service = new AchievementService(context, mockHttpContextAccessor.Object);

        var achievementId = Guid.NewGuid();
        var dto1 = new CreateAchievementApproveDto { IsApproved = true };
        var dto2 = new CreateAchievementApproveDto { IsApproved = false };
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        // Act
        var result1 = await service.ApproveAsync(achievementId, userId1, dto1);
        var result2 = await service.ApproveAsync(achievementId, userId2, dto2);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.True(result1.IsApproved);
        Assert.False(result2.IsApproved);
    }

    [Fact]
    public async Task GetApprovalSummaryAsync_NoVotes_ReturnsZeros()
    {
        // Arrange
        var mockHttpContextAccessor = CreateMockHttpContextAccessor();
        var context = CreateInMemoryContext();
        var service = new AchievementService(context, mockHttpContextAccessor.Object);
        var achievementId = Guid.NewGuid();

        // Act
        var summary = await service.GetApprovalSummaryAsync(achievementId);

        // Assert
        Assert.NotNull(summary);
        Assert.Equal(0, summary.Approved);
        Assert.Equal(0, summary.Denied);
    }

    [Fact]
    public async Task GetApprovalSummaryAsync_WithVotes_ReturnsCorrectCounts()
    {
        // Arrange
        var mockHttpContextAccessor = CreateMockHttpContextAccessor();
        var context = CreateInMemoryContext();
        var service = new AchievementService(context, mockHttpContextAccessor.Object);
        var achievementId = Guid.NewGuid();

        await service.ApproveAsync(achievementId, Guid.NewGuid(), new CreateAchievementApproveDto { IsApproved = true });
        await service.ApproveAsync(achievementId, Guid.NewGuid(), new CreateAchievementApproveDto { IsApproved = true });
        await service.ApproveAsync(achievementId, Guid.NewGuid(), new CreateAchievementApproveDto { IsApproved = false });

        // Act
        var result = await service.GetApprovalSummaryAsync(achievementId);

        // Assert
        Assert.Equal(2, result.Approved);
        Assert.Equal(1, result.Denied);
    }


    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsAchievement()
    {
        // Arrange
        var mockHttpContextAccessor = CreateMockHttpContextAccessor();
        var context = CreateInMemoryContext();
        var service = new AchievementService(context, mockHttpContextAccessor.Object);

        var dto = new CreateAchievementRequest
        {
            Title = "Test Achievement",
            Description = "Test Description"
        };

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(dto.Title, result.Value!.Title);
    }

    [Fact]
    public async Task DeleteAsync_ExistingAchievement_ReturnsTrueAndSetsDeletedAt()
    {
        // Arrange
        var mockHttpContextAccessor = CreateMockHttpContextAccessor();
        var context = CreateInMemoryContext();
        var service = new AchievementService(context, mockHttpContextAccessor.Object);

        var createdResult = await service.CreateAsync(new CreateAchievementRequest
        {
            Title = "To delete"
        });

        // Act
        var result = await service.DeleteAsync(createdResult.Value!.AchievementId);

        // Assert
        Assert.True(result.IsSuccess);
        var deleted = await context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == createdResult.Value!.AchievementId);
        Assert.NotNull(deleted!.DeletedAt);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingAchievement_ReturnsFalse()
    {
        // Arrange
        var mockHttpContextAccessor = CreateMockHttpContextAccessor();
        var context = CreateInMemoryContext();
        var service = new AchievementService(context, mockHttpContextAccessor.Object);

        // Act
        var result = await service.DeleteAsync(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyNonDeleted()
    {
        // Arrange
        var mockHttpContextAccessor = CreateMockHttpContextAccessor();
        var context = CreateInMemoryContext();
        var service = new AchievementService(context, mockHttpContextAccessor.Object);

        var achievementToStay = await service.CreateAsync(new CreateAchievementRequest
        {
            Title = "Active"
        });
        var achievementToDelete = await service.CreateAsync(new CreateAchievementRequest
        {
            Title = "Deleted"
        });
        await service.DeleteAsync(achievementToDelete.Value!.AchievementId);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.Equal("Active", result.Value![0].Title);
    }
}