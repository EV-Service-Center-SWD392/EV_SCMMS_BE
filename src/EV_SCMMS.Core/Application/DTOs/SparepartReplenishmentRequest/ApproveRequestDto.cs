using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.SparepartReplenishmentRequest;

/// <summary>
/// DTO for approving a replenishment request
/// </summary>
public class ApproveRequestDto
{
    /// <summary>
    /// User ID who is approving the request
    /// </summary>
    [Required(ErrorMessage = "ApprovedBy is required")]
    public Guid ApprovedBy { get; set; }

    /// <summary>
    /// Optional notes for approval
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}
