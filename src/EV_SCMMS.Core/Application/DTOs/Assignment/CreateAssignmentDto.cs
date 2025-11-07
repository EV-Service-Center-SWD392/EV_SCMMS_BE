using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Assignment;

/// <summary>
/// Create request DTO for technician assignment (AssignmentThaoNTT)
/// </summary>
public class CreateAssignmentDto
{
    [Required]
    public Guid BookingId { get; set; }

    [Required]
    public Guid TechnicianId { get; set; }

    [Required]
    public DateTime PlannedStartUtc { get; set; }

    [Required]
    public DateTime PlannedEndUtc { get; set; }
}
