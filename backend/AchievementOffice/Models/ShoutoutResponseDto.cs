namespace AchievementOffice.Models
{
    public class ShoutoutResponseDto
    {
        public Guid ShoutoutId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}