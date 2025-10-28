namespace EV_SCMMS.Core.Application.Settings;

/// <summary>
/// Configuration settings for work schedule management
/// </summary>
public class WorkScheduleSettings
{
    public int MaxTechniciansPerSchedule { get; set; } = 10;
    public int MinRestHoursBetweenShifts { get; set; } = 8;
    public int MaxWorkHoursPerDay { get; set; } = 8;
    public int MaxWorkHoursPerWeek { get; set; } = 40;
    public bool AllowOvertime { get; set; } = false;
    public int MaxOvertimeHoursPerDay { get; set; } = 2;
}