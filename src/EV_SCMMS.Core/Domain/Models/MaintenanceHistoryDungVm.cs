using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class MaintenanceHistoryDungVm
{
    public Guid MaintenanceHistoryDungVmid { get; set; }

    public Guid VehicleId { get; set; }

    public int? OrderId { get; set; }

    public DateTime? CompletedDate { get; set; }

    public string? Summary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual OrderThaoNtt? Order { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
