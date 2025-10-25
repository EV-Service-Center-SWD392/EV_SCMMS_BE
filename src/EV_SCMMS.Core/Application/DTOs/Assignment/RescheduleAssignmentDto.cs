using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Assignment;

public class RescheduleAssignmentDto
{
    [Required]
    public DateTime PlannedStartUtc { get; set; }

    [Required]
    public DateTime PlannedEndUtc { get; set; }

    public string? Reason { get; set; }
}

