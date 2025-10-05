namespace EV_SCMMS.Core.Application.DTOs;

/// <summary>
/// DTO for authentication result
/// </summary>
public class AuthResultDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public UserInfoDto? User { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}

/// <summary>
/// DTO for user information
/// </summary>
public class UserInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}
