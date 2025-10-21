using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Vehicleconditiondungvm
{
    public Guid Conditionid { get; set; }

    public Guid Vehicleid { get; set; }

    public DateTime? Lastmaintenance { get; set; }

    public string? Condition { get; set; }

    public int? Batteryhealth { get; set; }

    public int? Tirepressure { get; set; }

    public string? Brakestatus { get; set; }

    public string? Bodystatus { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
