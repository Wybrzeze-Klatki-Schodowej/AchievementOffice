namespace AchievementOffice.Entities;

public class UserRole
{ 
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public ICollection<User> Users { get; } = new HashSet<User>();
}