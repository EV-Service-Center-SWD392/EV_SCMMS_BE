using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkSchedule;

/// <summary>
/// Work schedule DTO representing a technician's work slot
/// </summary>
public class WorkScheduleDto
{
    public Guid Id { get; set; }

    [Required]
    public Guid TechnicianId { get; set; }

    [Required]
    public DateOnly WorkDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Range(0, int.MaxValue)]
    public int SlotCapacity { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

