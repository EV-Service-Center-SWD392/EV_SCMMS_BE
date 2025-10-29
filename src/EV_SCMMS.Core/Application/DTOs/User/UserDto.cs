namespace EV_SCMMS.Core.Application.DTOs.User;

/// <summary>
/// Data transfer object for user information
/// </summary>
public class UserDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User first name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// User last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// User full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// User phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// User birthday
    /// </summary>
    public DateOnly? Birthday { get; set; }

    /// <summary>
    /// User role ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// User role name
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// User status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// User active status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}