using System.ComponentModel.DataAnnotations.Schema;

namespace AchievementOffice.Entities
{
    [Table("KudosShoutouts")]
    public class KudosShoutout
    {
        [Column("shoutout_id")]
        public Guid ShoutoutId { get; set; }
        public Shoutout Shoutout { get; set; } = null!;

        [Column("user_id")]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Column("reaction")]
        public ReactionType Reaction { get; set; }
    }
}