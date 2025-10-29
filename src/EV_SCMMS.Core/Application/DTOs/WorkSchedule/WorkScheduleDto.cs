using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkSchedule;

/// <summary>
/// Work schedule DTO representing a work schedule
/// </summary>
public class WorkScheduleDto
{
    public Guid WorkScheduleId { get; set; }
    public Guid CenterId { get; set; }
    public DateTime Starttime { get; set; }
    public DateTime Endtime { get; set; }
    public string? Status { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

