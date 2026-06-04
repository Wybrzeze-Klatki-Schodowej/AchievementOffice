using AchievementOffice.Data;
using AchievementOffice.Entities;
using AchievementOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;

    public CommentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CommentResponse>> GetProfileCommentsAsync(Guid profileUserId)
    {
        return await _context.Comments
            .Include(c => c.Author)
                .ThenInclude(a => a.UserDetails)
            .Where(c =>
                c.ProfileUserId == profileUserId &&
                c.DeletedAt == null)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                AuthorLogin = c.Author.Login,
                AuthorFirstName = c.Author.UserDetails.Firstname,
                AuthorLastName = c.Author.UserDetails.Lastname,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<Result<CommentResponse>> CreateCommentAsync(
        Guid authorId, Guid profileUserId, CreateCommentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result<CommentResponse>.Fail("Comment content cannot be empty");
        }

        var author = await _context.Users
            .Include(u => u.UserDetails)
            .FirstOrDefaultAsync(
                u => u.Id == authorId && 
                    u.DeletedAt == null);

        if (author == null)
        {
            return Result<CommentResponse>.Fail("Author not found");
        }

        var profileUserExists = await _context.Users.AnyAsync(
            u => u.Id == profileUserId && 
                u.DeletedAt == null);

        if (!profileUserExists)
        {
            return Result<CommentResponse>.Fail("Profile user not found");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            ProfileUserId = profileUserId,
            Content = request.Content.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);

        await _context.SaveChangesAsync();

        var response = new CommentResponse
        {
            Id = comment.Id,
            AuthorLogin = author.Login,
            AuthorFirstName = author.UserDetails.Firstname,
            AuthorLastName = author.UserDetails.Lastname,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };

        return Result<CommentResponse>.Success(response);
    }

    public async Task<Result<CommentResponse>> UpdateCommentAsync(
        Guid commentId, Guid authorId, UpdateCommentRequest request)
    {

        var comment = await _context.Comments
            .Include(c => c.Author)
                .ThenInclude(a => a.UserDetails)
            .FirstOrDefaultAsync(
                c => c.Id == commentId && 
                    c.DeletedAt == null);

        if (comment == null)
        {
            return Result<CommentResponse>.Fail("Comment not found");
        }

        if (comment.AuthorId != authorId)
        {
            return Result<CommentResponse>.Fail("Unauthorized to edit this comment");
        }

        comment.Content = request.Content.Trim();
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = new CommentResponse
        {
            Id = comment.Id,
            AuthorLogin = comment.Author.Login,
            AuthorFirstName = comment.Author.UserDetails.Firstname,
            AuthorLastName = comment.Author.UserDetails.Lastname,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };

        return Result<CommentResponse>.Success(response);
    }

    public async Task<Result> DeleteCommentAsync(Guid commentId, Guid authorId)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(
                c => c.Id == commentId && 
                    c.DeletedAt == null);

        if (comment == null)
        {
            return Result.Fail("Comment not found");
        }

        if (comment.AuthorId != authorId)
        {
            return Result.Fail("Unauthorized to delete this comment");
        }

        comment.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result.Success();
    }
}