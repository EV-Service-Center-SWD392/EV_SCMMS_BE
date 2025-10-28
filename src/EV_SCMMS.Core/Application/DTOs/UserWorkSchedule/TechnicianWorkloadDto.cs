namespace EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;

/// <summary>
/// DTO for technician workload information
/// </summary>
public class TechnicianWorkloadDto
{
    public Guid TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public int AssignedSchedulesCount { get; set; }
    public double TotalWorkHours { get; set; }
    public DateTime Date { get; set; }
    public bool IsOverloaded { get; set; }
}