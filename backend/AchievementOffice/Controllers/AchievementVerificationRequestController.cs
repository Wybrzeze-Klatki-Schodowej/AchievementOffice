using System.Security.Claims;
using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Controllers;

[ApiController]
[Authorize]
public class AchievementVerificationRequestController
    : ControllerBase
{
    private readonly IAchievementVerificationRequestService
        _verificationRequestService;

    public AchievementVerificationRequestController(
        IAchievementVerificationRequestService verificationRequestService)
    {
        _verificationRequestService = verificationRequestService;
    }

    [HttpPost(
        "api/achievements/{achievementId:guid}/verification-requests")]
    public async Task<ActionResult<AchievementVerificationRequestResponse>>
        Create(
            Guid achievementId,
            CreateAchievementVerificationRequestDto request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = 
            await _verificationRequestService.CreateAsync(
                achievementId,
                userId,
                request);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.ErrorMessage
            });
        }

        return Ok(result.Value);
    }

    [HttpGet(
        "api/verification-requests/pending")]
    public async Task<ActionResult<List<AchievementVerificationRequestResponse>>>
        GetPending()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = 
            await _verificationRequestService.GetPendingForUserAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.ErrorMessage
            });
        }

        return Ok(result.Value);
    }

    [HttpPost(
        "api/verification-requests/{requestId:guid}/approve")]
    public async Task<ActionResult>
        Approve(Guid requestId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result =
            await _verificationRequestService
                .ApproveAsync(
                    requestId,
                    userId);

        if (!result.IsSuccess)
        {
            return result.ErrorMessage switch
            {
                "Verification request not found"
                    => NotFound(),

                "Forbidden."
                    => Forbid(),

                _ => BadRequest(new
                {
                    message = result.ErrorMessage
                })
            };
        }

        return Ok();
    }

    [HttpPost(
        "api/verification-requests/{requestId:guid}/reject")]
    public async Task<ActionResult>
        Reject(Guid requestId)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result =
            await _verificationRequestService
                .RejectAsync(
                    requestId,
                    userId);
        
        if (!result.IsSuccess)
        {
            return result.ErrorMessage switch
            {
                "Verification request not found"
                    => NotFound(),

                "Forbidden."
                    => Forbid(),

                _ => BadRequest(new
                {
                    message = result.ErrorMessage
                })
            };
        }

        return Ok();
    }

    [HttpGet(
        "api/achievements/{achievementId:guid}/available-reviewers")]
    public async Task<ActionResult<List<UserReviewerResponse>>>
        GetAvailableReviewers(
            Guid achievementId)
    {
        var result = 
            await _verificationRequestService
                .GetAvailableReviewersAsync(
                    achievementId);

        if (!result.IsSuccess)
        {
            return NotFound(new
            {
                message = result.ErrorMessage
            });
        }

        return Ok(result.Value);
    }

    [HttpGet(
        "api/verification-requests/{id:guid}")]
    public async Task<ActionResult<AchievementVerificationRequestResponse>>
        GetById(Guid id)
    {
        var result = await _verificationRequestService.GetByIdAsync(id);

        if (!result.IsSuccess)
        {
            return NotFound(new
            {
                message = result.ErrorMessage
            });
        }

        return Ok(result.Value);
    }
}