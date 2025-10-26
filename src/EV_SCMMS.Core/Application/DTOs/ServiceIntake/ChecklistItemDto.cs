namespace EV_SCMMS.Core.Application.DTOs.ServiceIntake;

/// <summary>
/// Read model for EV checklist catalog items
/// </summary>
public class ChecklistItemDto
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
}
