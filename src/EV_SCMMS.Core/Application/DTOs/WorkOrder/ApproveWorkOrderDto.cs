using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

public class ApproveWorkOrderDto
{
    [Required]
    public Guid WorkOrderId { get; set; }

    [Required]
    public Guid ApprovedBy { get; set; }

    public string? Note { get; set; }
}

