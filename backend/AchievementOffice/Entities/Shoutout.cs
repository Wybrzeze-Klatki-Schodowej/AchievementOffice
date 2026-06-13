using System.ComponentModel.DataAnnotations.Schema;

namespace AchievementOffice.Entities
{
    [Table("Shoutouts")]
    public class Shoutout
    {
        // Konieczne może być zaktualizowanie encji User lub Shoutout lub KudosShoutout. 
        [Column("shoutout_id")]
        public Guid ShoutoutId { get; set; }

        [Column("receiver_id")]
        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;

        [Column("sender_id")]
        public Guid SenderId { get; set; }
        public User Sender { get; set; } = null!;

        [Column("title")]
        public required string Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } 

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        public ICollection<KudosShoutout> Kudos { get; set; } = new List<KudosShoutout>();
    }
}