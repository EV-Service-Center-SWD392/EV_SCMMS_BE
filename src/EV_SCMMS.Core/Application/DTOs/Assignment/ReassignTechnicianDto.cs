using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Assignment;

public class ReassignTechnicianDto
{
    [Required]
    public Guid NewTechnicianId { get; set; }

    public string? Reason { get; set; }
}

