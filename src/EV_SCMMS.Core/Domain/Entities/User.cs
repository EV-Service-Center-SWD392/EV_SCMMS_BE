using EV_SCMMS.Core.Application.Enums;

namespace EV_SCMMS.Core.Domain.Entities;

/// <summary>
/// User entity for business logic
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public StatusEnum Status { get; set; } = StatusEnum.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public string? ApplicationUserId { get; set; }
}
