namespace AchievementOffice.Models
{
    public class CreateShoutoutRequest
    {
        public Guid ReceiverId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}