using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;

/// <summary>
/// DTO for auto-assigning technicians to work schedule
/// </summary>
public class AutoAssignRequestDto
{
    [Required]
    public Guid CenterId { get; set; }
    
    [Required]
    public string Shift { get; set; } = string.Empty;
    
    [Required]
    public DateTime WorkDate { get; set; }
    
    [Range(1, 50)]
    public int RequiredTechnicianCount { get; set; }
    
    public List<string>? RequiredSkills { get; set; }
}