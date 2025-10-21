using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.SparepartType;

/// <summary>
/// SparepartType update request DTO
/// </summary>
public class UpdateSparepartTypeDto
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }
    public bool IsActive { get; set; }
}