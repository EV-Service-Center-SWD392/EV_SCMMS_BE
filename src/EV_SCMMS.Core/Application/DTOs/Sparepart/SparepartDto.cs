using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Sparepart;

/// <summary>
/// Sparepart DTO for response
/// </summary>
public class SparepartDto
{
    public Guid SparepartId { get; set; }

    public int? VehicleModelId { get; set; }

    public Guid InventoryId { get; set; }

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

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public string? InventoryName { get; set; }
    public string? TypeName { get; set; }
}