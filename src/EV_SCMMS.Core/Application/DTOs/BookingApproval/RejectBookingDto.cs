namespace EV_SCMMS.Core.Application.DTOs.BookingApproval;

/// <summary>
/// Request payload for rejecting a booking request.
/// </summary>
public class RejectBookingDto
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
