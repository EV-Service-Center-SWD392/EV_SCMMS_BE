namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

public class UpdateWorkOrderDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? EstimatedAmount { get; set; }
    public List<WorkOrderLineCreateDto>? Lines { get; set; }
}

