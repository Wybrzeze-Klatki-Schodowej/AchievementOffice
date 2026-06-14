namespace AchievementOffice.Models
{
    public class ShoutoutResponse
    {
        public Guid ShoutoutId { get; set; }
        
        public Guid SenderId { get; set; }

        public string SenderLogin { get; set; } = string.Empty;

        public string SenderFirstname { get; set; } = string.Empty;

        public string SenderLastname { get; set; } = string.Empty;

        public Guid ReceiverId { get; set; }

        public string ReceiverLogin { get; set; } = string.Empty;

        public string ReceiverFirstname { get; set; } = string.Empty;

        public string ReceiverLastname { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Dictionary<string, int> Reactions { get; set; } = new();
        public string? CurrentUserReaction { get; set; }

        public int VisibilityId { get; set; }
        public List<Guid> GroupIds { get; set; } = new();
    }
}
