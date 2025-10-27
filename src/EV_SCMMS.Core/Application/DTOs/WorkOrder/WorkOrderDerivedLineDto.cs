namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

/// <summary>
/// Response-only derived Work Order line built from checklist responses.
/// All fields are optional; no input validation attributes.
/// </summary>
public class WorkOrderDerivedLineDto
{
    public string? PartName { get; set; }
    public int? Qty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? LaborCode { get; set; }
    public decimal? LaborHours { get; set; }
}

