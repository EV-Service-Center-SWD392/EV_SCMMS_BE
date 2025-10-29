using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.SparepartUsageHistory;

/// <summary>
/// SparepartUsageHistory creation request DTO
/// </summary>
public class CreateSparepartUsageHistoryDto
{
    [Required]
    public Guid SparepartId { get; set; }

    [Required]
    public Guid CenterId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity used must be greater than 0")]
    public int QuantityUsed { get; set; }

    [Required]
    [StringLength(100)]
    public string UsedBy { get; set; } = string.Empty;

    [StringLength(50)]
    public string? VehicleId { get; set; }

    [StringLength(50)]
    public string? WorkOrderId { get; set; }

    [StringLength(100)]
    public string? MaintenanceType { get; set; }

    [StringLength(200)]
    public string? Purpose { get; set; }

    [Required]
    public DateTime UsageDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}