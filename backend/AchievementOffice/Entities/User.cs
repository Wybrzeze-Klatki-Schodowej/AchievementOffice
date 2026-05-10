namespace AchievementOffice.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Login { get; set; }
        public required string Password { get; set; }
        public string? LastPassword { get; set; }
        public required string Email { get; set; }
        public string? LastEmail { get; set; }
        public Guid UserRoleId { get; set; }
        public UserRole UserRole { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
