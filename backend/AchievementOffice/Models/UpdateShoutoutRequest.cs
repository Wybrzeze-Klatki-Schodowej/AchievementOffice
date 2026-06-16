namespace AchievementOffice.Models
{
    public class UpdateShoutoutRequest
    {
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }

        public int VisibilityId { get; set; }
        public List<Guid>? GroupIds { get; set; } = new();
    }
}