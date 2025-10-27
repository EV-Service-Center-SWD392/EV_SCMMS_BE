using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.ChecklistItems;

public class CreateChecklistItemDto
{
    [MaxLength(50)]
    public string? Code { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 3)]
    public short Type { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; }
}

