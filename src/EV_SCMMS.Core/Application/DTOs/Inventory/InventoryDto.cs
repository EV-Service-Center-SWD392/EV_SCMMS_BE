using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Inventory;

/// <summary>
/// Inventory DTO for response
/// </summary>
public class InventoryDto
{
    public Guid InventoryId { get; set; }

    public Guid CenterId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    public int? MinimumStockLevel { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public string? CenterName { get; set; }
}