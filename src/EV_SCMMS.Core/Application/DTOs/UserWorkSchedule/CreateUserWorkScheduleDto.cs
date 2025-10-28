using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;

/// <summary>
/// DTO for creating UserWorkSchedule
/// </summary>
public class CreateUserWorkScheduleDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid WorkScheduleId { get; set; }
    
    public string? Status { get; set; } = "Active";
}