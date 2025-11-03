namespace EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;

public class TechnicianScheduleDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public List<WorkScheduleAssignmentDto> Schedules { get; set; } = new();
}

public class WorkScheduleAssignmentDto
{
    public Guid UserWorkScheduleId { get; set; }
    public Guid WorkScheduleId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string CenterName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
}