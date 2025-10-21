using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Sparepart;

/// <summary>
/// Sparepart update request DTO
/// </summary>
public class UpdateSparepartDto
{
    public int? VehicleModelId { get; set; }

    [Required]
    public Guid InventoryId { get; set; }

    [Required]
    public Guid TypeId { get; set; }

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

    [StringLength(50)]
    public string? Status { get; set; }
    public bool IsActive { get; set; }
}