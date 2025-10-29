using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

/// <summary>
/// Request payload for a Work Order line (input only).
/// Server derives PartName and UnitPrice from SparepartTuht.
/// </summary>
public class WorkOrderLineCreateDto
{
    [Required]
    public Guid PartId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Qty must be at least 1")]
    public int Qty { get; set; }

    public string? LaborCode { get; set; }

    [Range(0, 1000)]
    public decimal? LaborHours { get; set; }
}

