using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

/// <summary>
/// Line item inside a Work Order draft (not persisted on current entity model).
/// </summary>
public class WorkOrderLineDto
{
    [Required]
    public Guid? PartId { get; set; }

    // Auto-filled from SparepartTuht if missing (server-side)
    public string? PartName { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Qty must be at least 1")]
    public int? Qty { get; set; }

    // Auto-filled from SparepartTuht if missing (server-side)
    public decimal? UnitPrice { get; set; }

    public string? LaborCode { get; set; }

    [Range(0, 1000)]
    public decimal? LaborHours { get; set; }
}

