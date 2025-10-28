using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

public class ApproveWorkOrderDto
{
    [System.ComponentModel.DataAnnotations.MaxLength(1000)]
    public string? Note { get; set; }
}

