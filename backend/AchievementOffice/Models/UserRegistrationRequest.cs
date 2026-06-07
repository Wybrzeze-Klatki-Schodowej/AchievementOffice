using System.ComponentModel.DataAnnotations;

namespace AchievementOffice.Models;

public class UserRegistrationRequest
{
    [Required(ErrorMessage = "Email cannot be empty")]
    [RegularExpression(RegexPatterns.Email, ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    [RegularExpression(@"^\S+$", ErrorMessage = "Username cannot contain spaces")]
    [Required(ErrorMessage = "Username cannot be empty")]
    public required string Username { get; set; }

    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "First name cannot be empty")]
    public required string Firstname { get; set; }

    [Required(ErrorMessage = "Last name cannot be empty")]
    public required string Lastname { get; set; }

    [Required(ErrorMessage = "Job title cannot be empty")]
    public required string JobTitle { get; set; }
    public string? RoleName { get; set; }
}