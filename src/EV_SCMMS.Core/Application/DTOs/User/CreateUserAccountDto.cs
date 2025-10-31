using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.User;

/// <summary>
/// DTO for creating UserAccount
/// </summary>
public class CreateUserAccountDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public Guid RoleId { get; set; }
}