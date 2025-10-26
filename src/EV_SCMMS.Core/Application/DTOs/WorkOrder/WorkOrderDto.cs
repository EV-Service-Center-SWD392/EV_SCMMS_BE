namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

public class WorkOrderDto
{
    public Guid Id { get; set; }
    public Guid IntakeId { get; set; }
    public string Status { get; set; } = string.Empty;

    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? EstimatedAmount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public List<WorkOrderLineDto>? Lines { get; set; }
}
