using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Sparepart;

/// <summary>
/// Sparepart create request DTO
/// </summary>
public class CreateSparepartDto
{
    public int? VehicleModelId { get; set; }

    [Required]
    [StringLength(255)]
    public string CenterName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string TypeName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Manufacturer { get; set; }

    [StringLength(100)]
    public string? PartNumber { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}