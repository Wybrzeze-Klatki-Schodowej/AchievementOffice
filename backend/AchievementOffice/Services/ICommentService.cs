using AchievementOffice.Models;

namespace AchievementOffice.Services;

public interface ICommentService
{
    Task<List<CommentResponse>> GetProfileCommentsAsync(Guid profileUserId);

    Task<Result<CommentResponse>> CreateCommentAsync(
        Guid authorId, Guid profileUserId, CreateCommentRequest request);

    Task<Result<CommentResponse>> UpdateCommentAsync(
        Guid commentId, Guid authorId, UpdateCommentRequest request);

    Task<Result> DeleteCommentAsync(Guid commentId, Guid authorId);
}