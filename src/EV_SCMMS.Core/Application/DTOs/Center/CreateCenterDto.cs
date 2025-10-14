using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Center;

/// <summary>
/// Center create request DTO
/// </summary>
public class CreateCenterDto
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }
}