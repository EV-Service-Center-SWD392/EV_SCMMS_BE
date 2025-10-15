using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Auth;

/// <summary>
/// Data transfer object for user registration
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirm password
    /// </summary>
    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// User first name
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [MaxLength(128, ErrorMessage = "First name cannot exceed 128 characters")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User last name
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(128, ErrorMessage = "Last name cannot exceed 128 characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User phone number
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(10, ErrorMessage = "Phone number cannot exceed 32 characters")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User address
    /// </summary>
    [MaxLength(256, ErrorMessage = "Address cannot exceed 256 characters")]
    public string? Address { get; set; }

    /// <summary>
    /// User birthday
    /// </summary>
    public DateOnly? Birthday { get; set; }


}