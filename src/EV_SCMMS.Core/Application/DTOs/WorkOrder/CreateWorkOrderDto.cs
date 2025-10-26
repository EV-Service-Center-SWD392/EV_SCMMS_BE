using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

public class CreateWorkOrderDto
{
    [Required]
    public Guid IntakeId { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? EstimatedAmount { get; set; }
    public List<WorkOrderLineDto>? Lines { get; set; }
}
