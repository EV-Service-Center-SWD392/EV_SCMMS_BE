namespace EV_SCMMS.Core.Application.DTOs.ServiceIntake;

/// <summary>
/// Intake-specific response payload for checklist items
/// </summary>
public class ChecklistResponseItemDto
{
    public Guid ItemId { get; set; }
    public string? Value { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(1000)]
    public string? Note { get; set; }
    public string? PhotoUrl { get; set; }
}
