using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

public class RejectWorkOrderDto
{
    [Required]
    public Guid WorkOrderId { get; set; }

    [Required]
    public Guid RejectedBy { get; set; }

    [Required]
    public string Reason { get; set; } = string.Empty;
}

