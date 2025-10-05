using Microsoft.AspNetCore.Identity;

namespace EV_SCMMS.Infrastructure.Identity;

/// <summary>
/// Application user entity extending IdentityUser
/// This is an infrastructure concern for ASP.NET Core Identity
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
