using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class VehicleConditionDungVm
{
    public Guid VehicleConditionDungVmid { get; set; }

    public Guid VehicleId { get; set; }

    public DateTime? LastMaintenance { get; set; }

    public string? Condition { get; set; }

    public string? Status { get; set; }

    public int? BatteryHealth { get; set; }

    public int? TirePressure { get; set; }

    public string? BrakeStatus { get; set; }

    public string? BodyStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
