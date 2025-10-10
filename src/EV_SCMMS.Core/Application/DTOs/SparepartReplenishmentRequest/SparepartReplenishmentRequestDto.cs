using System.ComponentModel.DataAnnotations;
using EV_SCMMS.Core.Application.DTOs.Sparepart;
using EV_SCMMS.Core.Application.DTOs.Center;

namespace EV_SCMMS.Core.Application.DTOs.SparepartReplenishmentRequest;

/// <summary>
/// SparepartReplenishmentRequest response DTO
/// </summary>
public class SparepartReplenishmentRequestDto
{
    public Guid Id { get; set; }

    public Guid SparepartId { get; set; }

    public Guid CenterId { get; set; }

    public int RequestedQuantity { get; set; }

    public string RequestedBy { get; set; } = string.Empty;

    public string? ApprovedBy { get; set; }

    public string? SupplierId { get; set; }

    public decimal? EstimatedCost { get; set; }

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime RequestDate { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public DateTime? ActualDeliveryDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation properties
    public virtual SparepartDto? Sparepart { get; set; }

    public virtual CenterDto? Center { get; set; }
}