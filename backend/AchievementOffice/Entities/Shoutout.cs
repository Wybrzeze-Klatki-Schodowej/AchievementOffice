namespace AchievementOffice.Entities
{
    public class Shoutout
    {
        // Konieczne może być zaktualizowanie encji User lub Shoutout lub KudosShoutout. 
        public Guid ShoutoutId { get; set; }

        public required Guid ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;

        public required Guid SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public required string Title { get; set; }
        public string ? Description { get; set; }

        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; } 
        public DateTime? DeletedAt { get; set; }
    }
}