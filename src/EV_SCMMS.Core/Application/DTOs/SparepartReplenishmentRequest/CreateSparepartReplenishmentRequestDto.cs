using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.SparepartReplenishmentRequest;

/// <summary>
/// SparepartReplenishmentRequest creation request DTO
/// </summary>
public class CreateSparepartReplenishmentRequestDto
{
    [Required]
    public Guid SparepartId { get; set; }

    [Required]
    public Guid CenterId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Requested quantity must be greater than 0")]
    public int RequestedQuantity { get; set; }

    [Required]
    [StringLength(100)]
    public string RequestedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? SupplierId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Estimated cost must be non-negative")]
    public decimal? EstimatedCost { get; set; }

    [Required]
    [StringLength(50)]
    public string Priority { get; set; } = "Medium";

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }
    public string Status { get; set; } = "PENDING";
}