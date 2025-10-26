namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

/// <summary>
/// Line item inside a Work Order draft (not persisted on current entity model).
/// </summary>
public class WorkOrderLineDto
{
    public Guid? PartId { get; set; }
    public string? PartName { get; set; }
    public int? Qty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? LaborCode { get; set; }
    public decimal? LaborHours { get; set; }
}

