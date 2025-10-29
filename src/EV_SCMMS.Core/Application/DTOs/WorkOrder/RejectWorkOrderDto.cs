using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

public class RejectWorkOrderDto
{
    [Required]
    public string Reason { get; set; } = string.Empty;
}

