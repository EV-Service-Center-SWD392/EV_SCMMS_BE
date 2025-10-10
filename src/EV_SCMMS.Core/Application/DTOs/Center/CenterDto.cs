using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Center;

/// <summary>
/// Center DTO for response
/// </summary>
public class CenterDto
{
    public Guid CenterId { get; set; }

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

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}