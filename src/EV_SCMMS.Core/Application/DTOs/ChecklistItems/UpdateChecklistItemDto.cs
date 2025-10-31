using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.ChecklistItems;

public class UpdateChecklistItemDto
{
    [MaxLength(50)]
    public string? Code { get; set; }

    [MaxLength(200)]
    public string? Name { get; set; }

    [Range(1, 3)]
    public short? Type { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; }

    [RegularExpression("^(ACTIVE|INACTIVE)$", ErrorMessage = "Status must be ACTIVE or INACTIVE")]
    public string? Status { get; set; }

    public bool? IsActive { get; set; }
}

