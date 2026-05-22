using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Models;

public class UserRegistrationRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Username { get; set; }

    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public required string Password { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public required string JobTitle { get; set; }
    public string? RoleName { get; set; }
}