namespace AchievementOffice.Entities
{
    public class Shoutout
    {
        // Konieczne może być zaktualizowanie encji User lub Shoutout lub KudosShoutout. 
        public Guid ShoutoutId { get; set; }

        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;

        public Guid SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public required string Title { get; set; }
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } 
        public DateTime? DeletedAt { get; set; }
    }
}