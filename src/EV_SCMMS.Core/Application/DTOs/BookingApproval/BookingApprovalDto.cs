namespace EV_SCMMS.Core.Application.DTOs.BookingApproval;

/// <summary>
/// Booking representation for staff approval workflow.
/// </summary>
public class BookingApprovalDto
{
    public Guid Id { get; set; }
    public Guid? CenterId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid? ServiceTypeId { get; set; }
    public DateTime? PreferredStartUtc { get; set; }
    public DateTime? PreferredEndUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public DateTime? CreatedAt { get; set; }
}
