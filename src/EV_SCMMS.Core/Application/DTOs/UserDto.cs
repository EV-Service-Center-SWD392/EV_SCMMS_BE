namespace EV_SCMMS.Core.Application.DTOs;

/// <summary>
/// Data Transfer Object for User
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
