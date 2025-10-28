using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkSchedule;

/// <summary>
/// Create request DTO for work schedule
/// </summary>
public class CreateWorkScheduleDto
{
    [Required]
    public Guid CenterId { get; set; }

    [Required]
    public DateTime Starttime { get; set; }

    [Required]
    public DateTime Endtime { get; set; }

    public string? Status { get; set; } = "Active";
}

