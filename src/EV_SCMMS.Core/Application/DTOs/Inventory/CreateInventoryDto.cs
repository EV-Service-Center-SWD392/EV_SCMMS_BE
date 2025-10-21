using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Inventory;

/// <summary>
/// Inventory create request DTO
/// </summary>
public class CreateInventoryDto
{
    [Required]
    public Guid CenterId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    [Range(0, int.MaxValue)]
    public int? MinimumStockLevel { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }
}