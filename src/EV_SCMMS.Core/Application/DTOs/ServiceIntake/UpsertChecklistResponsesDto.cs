using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.ServiceIntake;

/// <summary>
/// Payload for bulk upsert of checklist responses on an intake
/// </summary>
public class UpsertChecklistResponsesDto
{
    [Required]
    public Guid IntakeId { get; set; }

    [Required]
    public List<ChecklistResponseItemDto> Responses { get; set; } = new();
}
