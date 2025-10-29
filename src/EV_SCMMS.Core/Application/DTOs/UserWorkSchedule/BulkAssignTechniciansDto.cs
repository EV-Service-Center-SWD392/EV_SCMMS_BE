using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;

/// <summary>
/// DTO for bulk assigning technicians to work schedule
/// </summary>
public class BulkAssignTechniciansDto
{
    [Required]
    public Guid WorkScheduleId { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<Guid> TechnicianIds { get; set; } = new();
}