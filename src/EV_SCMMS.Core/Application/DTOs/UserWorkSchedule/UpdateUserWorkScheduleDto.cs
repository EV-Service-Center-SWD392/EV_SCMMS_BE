namespace EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;

/// <summary>
/// DTO for updating UserWorkSchedule
/// </summary>
public class UpdateUserWorkScheduleDto
{
    public string? Status { get; set; }
    public bool IsActive { get; set; } = true;
}