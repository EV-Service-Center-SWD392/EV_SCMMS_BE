using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.WorkOrder;

public class ReviseWorkOrderDto
{
    [Required]
    public UpdateWorkOrderDto Payload { get; set; } = new();
}

