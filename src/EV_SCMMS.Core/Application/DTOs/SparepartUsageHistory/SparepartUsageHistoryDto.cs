using System.ComponentModel.DataAnnotations;
using EV_SCMMS.Core.Application.DTOs.Sparepart;
using EV_SCMMS.Core.Application.DTOs.Center;

namespace EV_SCMMS.Core.Application.DTOs.SparepartUsageHistory;

/// <summary>
/// SparepartUsageHistory response DTO
/// </summary>
public class SparepartUsageHistoryDto
{
    public Guid Id { get; set; }

    public Guid SparepartId { get; set; }

    public Guid CenterId { get; set; }

    public int QuantityUsed { get; set; }

    public string UsedBy { get; set; } = string.Empty;

    public string? VehicleId { get; set; }

    public string? WorkOrderId { get; set; }

    public string? MaintenanceType { get; set; }

    public string? Purpose { get; set; }

    public DateTime UsageDate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation properties
    public virtual SparepartDto? Sparepart { get; set; }

    public virtual CenterDto? Center { get; set; }
}