using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EV_SCMMS.Core.Application.DTOs.ServiceIntake;

/// <summary>
/// Payload for bulk upsert of checklist responses on an intake
/// </summary>
public class UpsertChecklistResponsesDto
{
    // Populated from the route in controller; not required in body
    [JsonIgnore]
    public Guid IntakeId { get; set; }

    // Optional common note for this submission
    public string? Note { get; set; }

    [Required]
    public List<ChecklistResponseItemDto> Responses { get; set; } = new();
}
