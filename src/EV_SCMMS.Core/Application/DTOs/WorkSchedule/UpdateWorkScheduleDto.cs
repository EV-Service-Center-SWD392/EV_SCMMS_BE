using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkSchedule;

/// <summary>
/// Update request DTO for technician work schedule
/// </summary>
public class UpdateWorkScheduleDto
{
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
}

