namespace AchievementOffice.Entities;

public class Comment
{
    public Guid Id { get; set; }
    
    public Guid AuthorId { get; set; }

    public User Author { get; set; } = null!;

    public Guid ProfileUserId { get; set; }

    public User ProfileUser { get; set; } = null!;

    public required string Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    
    public DateTime? DeletedAt { get; set; }
}