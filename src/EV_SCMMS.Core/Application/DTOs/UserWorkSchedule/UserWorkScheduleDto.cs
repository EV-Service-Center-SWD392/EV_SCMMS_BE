namespace EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;

/// <summary>
/// DTO for UserWorkSchedule entity
/// </summary>
public class UserWorkScheduleDto
{
    public Guid UserWorkScheduleId { get; set; }
    public Guid UserId { get; set; }
    public Guid WorkScheduleId { get; set; }
    public string? Status { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public string? UserName { get; set; }
    public DateTime? WorkScheduleStartTime { get; set; }
    public DateTime? WorkScheduleEndTime { get; set; }
}