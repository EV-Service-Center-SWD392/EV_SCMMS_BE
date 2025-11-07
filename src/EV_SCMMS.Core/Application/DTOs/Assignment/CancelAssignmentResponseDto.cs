namespace EV_SCMMS.Core.Application.DTOs.Assignment;

/// <summary>
/// Response DTO for assignment cancellation
/// Provides information about the cancelled assignment and booking status
/// </summary>
public class CancelAssignmentResponseDto
{
    /// <summary>
    /// ID of the cancelled assignment
    /// </summary>
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// ID of the associated booking
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// Indicates if the booking has any remaining active assignments
    /// False means the booking is now available for reassignment
    /// </summary>
    public bool HasActiveAssignments { get; set; }

    /// <summary>
    /// Current status of the booking after assignment cancellation
    /// </summary>
    public string BookingStatus { get; set; } = string.Empty;

    /// <summary>
    /// User-friendly message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
