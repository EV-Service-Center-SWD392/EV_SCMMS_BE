using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.SparepartReplenishmentRequest;

/// <summary>
/// DTO for rejecting a replenishment request
/// </summary>
public class RejectRequestDto
{
    /// <summary>
    /// User ID who is rejecting the request
    /// </summary>
    [Required(ErrorMessage = "RejectedBy is required")]
    public Guid RejectedBy { get; set; }

    /// <summary>
    /// Reason for rejection
    /// </summary>
    [Required(ErrorMessage = "Rejection reason is required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Reason must be between 10 and 500 characters")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Optional additional notes
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}
