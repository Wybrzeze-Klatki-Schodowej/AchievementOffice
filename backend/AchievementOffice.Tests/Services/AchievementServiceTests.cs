using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Services;
using AchievementOffice.Models;

namespace AchievementOffice.Tests;

public class AchievementServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly AchievementService _service;

    public AchievementServiceTests()
    {
        _context = CreateInMemoryContext();
        _mockHttpContextAccessor = CreateMockHttpContextAccessor();
        _service = new AchievementService(
            _context,
            _mockHttpContextAccessor.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IRankingService>());
    }

    private async Task SeedAchievementAsync(Guid achievementId)
    {
        _context.Achievements.Add(new Achievement
        {
            AchievementId = achievementId,
            UserId = Guid.NewGuid(),
            Title = "Test achievement"
        });
        await _context.SaveChangesAsync();
    }

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

    [Test]
    public async Task ApproveAsync_FirstVote_ReturnsDtoWithCorrectStatus()
    {
        // Arrange

        var userId = Guid.NewGuid();
        var achievementId = Guid.NewGuid();
        await SeedAchievementAsync(achievementId);
        var dto = new CreateAchievementApproveDto
        {
            IsApproved = true
        };

        // Act
        var result = await _service.ApproveAsync(achievementId, userId, dto);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.AchievementId).IsEqualTo(achievementId);
        await Assert.That(result.UserId).IsEqualTo(userId);
        await Assert.That(result.IsApproved).IsTrue();

    }

    [Test]
    public async Task ApproveAsync_SecondSameVote_PerformsSoftDeleteAndReturnsNullApproved()
    {
        // Arrange

        var userId = Guid.NewGuid();
        var achievementId = Guid.NewGuid();
        await SeedAchievementAsync(achievementId);
        var dto = new CreateAchievementApproveDto
        {
            IsApproved = true
        };

        await _service.ApproveAsync(achievementId, userId, dto);

        // Act
        var result = await _service.ApproveAsync(achievementId, userId, dto);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.IsApproved).IsNull();

        var dbRecord = await _context.AchievementApproves.IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.AchievementId == achievementId && a.UserId == userId);
        await Assert.That(dbRecord!.DeletedAt).IsNotNull();
    }

    [Test]
    public async Task ApproveAsync_DifferentUsers_Success()
    {
        // Arrange

        var achievementId = Guid.NewGuid();
        await SeedAchievementAsync(achievementId);
        var dto1 = new CreateAchievementApproveDto { IsApproved = true };
        var dto2 = new CreateAchievementApproveDto { IsApproved = false };
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        // Act
        var result1 = await _service.ApproveAsync(achievementId, userId1, dto1);
        var result2 = await _service.ApproveAsync(achievementId, userId2, dto2);

        // Assert
        await Assert.That(result1).IsNotNull();
        await Assert.That(result2).IsNotNull();
        await Assert.That(result1.AchievementId).IsEqualTo(achievementId);
        await Assert.That(result2.AchievementId).IsEqualTo(achievementId);
        await Assert.That(result1.UserId).IsEqualTo(userId1);
        await Assert.That(result2.UserId).IsEqualTo(userId2);
        await Assert.That(result1.IsApproved).IsTrue();
        await Assert.That(result2.IsApproved).IsFalse();
    }

    [Test]
    public async Task GetApprovalSummaryAsync_NoVotes_ReturnsZeros()
    {
        // Arrange
        var achievementId = Guid.NewGuid();
        await SeedAchievementAsync(achievementId);

        // Act
        var summary = await _service.GetApprovalSummaryAsync(achievementId);

        // Assert
        await Assert.That(summary).IsNotNull();
        await Assert.That(summary.Approved).IsEqualTo(0);
        await Assert.That(summary.Denied).IsEqualTo(0);
    }

    [Test]
    public async Task GetApprovalSummaryAsync_WithVotes_ReturnsCorrectCounts()
    {
        // Arrange
        var achievementId = Guid.NewGuid();
        await SeedAchievementAsync(achievementId);

        await _service.ApproveAsync(achievementId, Guid.NewGuid(), new CreateAchievementApproveDto { IsApproved = true });
        await _service.ApproveAsync(achievementId, Guid.NewGuid(), new CreateAchievementApproveDto { IsApproved = true });
        await _service.ApproveAsync(achievementId, Guid.NewGuid(), new CreateAchievementApproveDto { IsApproved = false });

        // Act
        var result = await _service.GetApprovalSummaryAsync(achievementId);

        // Assert
        await Assert.That(result.Approved).IsEqualTo(2);
        await Assert.That(result.Denied).IsEqualTo(1);
    }


    [Test]
    public async Task CreateAsync_ValidDto_ReturnsAchievement()
    {
        // Arrange

        var dto = new CreateAchievementRequest
        {
            Title = "Test Achievement",
            Description = "Test Description"
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value!.Title).IsEqualTo(dto.Title);
    }

    [Test]
    public async Task DeleteAsync_ExistingAchievement_ReturnsTrueAndSetsDeletedAt()
    {
        // Arrange

        var createdResult = await _service.CreateAsync(new CreateAchievementRequest
        {
            Title = "To delete"
        });

        // Act
        var result = await _service.DeleteAsync(createdResult.Value!.AchievementId);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        var deleted = await _context.Achievements
            .FirstOrDefaultAsync(a => a.AchievementId == createdResult.Value!.AchievementId);
        await Assert.That(deleted!.DeletedAt).IsNotNull();
    }

    [Test]
    public async Task DeleteAsync_NonExistingAchievement_ReturnsFalse()
    {
        // Arrange


        // Act
        var result = await _service.DeleteAsync(Guid.NewGuid());

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
    }

    [Test]
    public async Task GetAllAsync_ReturnsOnlyNonDeleted()
    {
        // Arrange

        var achievementToStay = await _service.CreateAsync(new CreateAchievementRequest
        {
            Title = "Active"
        });
        var achievementToDelete = await _service.CreateAsync(new CreateAchievementRequest
        {
            Title = "Deleted"
        });
        await _service.DeleteAsync(achievementToDelete.Value!.AchievementId);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value!).Count().IsEqualTo(1);
        await Assert.That(result.Value![0].Title).IsEqualTo("Active");
    }
}