namespace EV_SCMMS.Core.Application.DTOs.BookingApproval;

/// <summary>
/// Request payload for approving a booking request.
/// </summary>
public class ApproveBookingDto
{
    public Guid BookingId { get; set; }
    public Guid StaffId { get; set; }
    public string? Note { get; set; }
}
