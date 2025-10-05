using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs;

/// <summary>
/// DTO for token refresh
/// </summary>
public class RefreshTokenDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
