using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AchievementOffice.Controllers;

[ApiController]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("api/users/{userId:guid}/comments")]
    public async Task<IActionResult> GetProfileComments(Guid userId)
    {
        var comments = await _commentService.GetProfileCommentsAsync(userId);

        return Ok(comments);
    }

    [HttpPost("api/users/{userId:guid}/comments")]
    public async Task<IActionResult> CreateComment(Guid userId, CreateCommentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        var authorId = Guid.Parse(userIdClaim.Value);

        var result = await _commentService.CreateCommentAsync(authorId, userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(new 
            { 
                Message = result.ErrorMessage 
            });
        }

        return Ok(result.Value);
    }

    [HttpPut("api/comments/{commentId:guid}")]
    public async Task<IActionResult> UpdateComment(Guid commentId, UpdateCommentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        var authorId = Guid.Parse(userIdClaim.Value);

        var result = await _commentService.UpdateCommentAsync(commentId, authorId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(new 
            { 
                Message = result.ErrorMessage 
            });
        }

        return Ok(result.Value);
    }

    [HttpDelete("api/comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        var authorId = Guid.Parse(userIdClaim.Value);

        var result = await _commentService.DeleteCommentAsync(commentId, authorId);

        if (!result.IsSuccess)
        {
            return BadRequest(new 
            { 
                Message = result.ErrorMessage 
            });
        }

        return Ok(new 
        { 
            Message = "Comment deleted successfully" 
        });
    }
}