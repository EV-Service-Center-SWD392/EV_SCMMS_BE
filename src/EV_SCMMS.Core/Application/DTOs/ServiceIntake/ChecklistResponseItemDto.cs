namespace EV_SCMMS.Core.Application.DTOs.ServiceIntake;

/// <summary>
/// Intake-specific response payload for checklist items
/// </summary>
public class ChecklistResponseItemDto
{
    public Guid ItemId { get; set; }
    public string? Value { get; set; }
    public string? Note { get; set; }
    public string? PhotoUrl { get; set; }
}
