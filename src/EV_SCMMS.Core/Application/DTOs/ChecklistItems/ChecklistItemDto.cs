using System;

namespace EV_SCMMS.Core.Application.DTOs.ChecklistItems;

public class ChecklistItemDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public short Type { get; set; }
    public string? Unit { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

