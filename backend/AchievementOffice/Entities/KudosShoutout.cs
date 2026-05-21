namespace AchievementOffice.Entities
{
    public class KudosShoutout
    {
        public Guid ShoutoutId { get; set; }
        public Shoutout Shoutout { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public ReactionType Reaction { get; set; }
    }
}