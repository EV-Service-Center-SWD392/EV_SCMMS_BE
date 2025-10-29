using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.User;

/// <summary>
/// DTO for updating UserAccount
/// </summary>
public class UpdateUserAccountDto
{
    [EmailAddress]
    public string? Email { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
    public bool IsActive { get; set; } = true;
}