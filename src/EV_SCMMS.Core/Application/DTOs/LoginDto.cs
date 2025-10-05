using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs;

/// <summary>
/// DTO for user login
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "Email or Username is required")]
    public string EmailOrUsername { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = false;
}
