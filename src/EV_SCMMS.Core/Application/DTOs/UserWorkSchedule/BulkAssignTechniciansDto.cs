using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;

/// <summary>
/// DTO for bulk assigning technicians to work schedule
/// </summary>
public class BulkAssignTechniciansDto
{
    [Required]
    public Guid CenterId { get; set; }
    
    [Required]
    public string Shift { get; set; } = string.Empty;
    
    [Required]
    public DateTime WorkDate { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<Guid> TechnicianIds { get; set; } = new();
}