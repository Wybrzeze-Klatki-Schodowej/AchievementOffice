namespace AchievementOffice.Models;

public class CommentResponse
{
    public Guid Id { get; set; }

    public Guid AuthorId { get; set; }
    
    public string AuthorLogin { get; set; } = string.Empty;

    public string AuthorFirstName { get; set; } = string.Empty;

    public string AuthorLastName { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}