using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class MaintenanceTaskDungVm
{
    public Guid MaintenanceTaskDungVmid { get; set; }

    public int OrderServiceId { get; set; }

    public Guid? TechnicianId { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? Task { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual OrderServiceThaoNtt OrderService { get; set; } = null!;

    public virtual User? Technician { get; set; }
}
