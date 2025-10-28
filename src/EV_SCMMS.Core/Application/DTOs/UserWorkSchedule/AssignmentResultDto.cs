namespace EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;

/// <summary>
/// DTO for bulk assignment result
/// </summary>
public class AssignmentResultDto
{
    public List<UserWorkScheduleDto> SuccessfulAssignments { get; set; } = new();
    public List<AssignmentErrorDto> FailedAssignments { get; set; } = new();
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
}