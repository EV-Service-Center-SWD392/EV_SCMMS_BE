using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

public class CompleteWorkDto
{
    [Required]
    public Guid WorkOrderId { get; set; }
}

