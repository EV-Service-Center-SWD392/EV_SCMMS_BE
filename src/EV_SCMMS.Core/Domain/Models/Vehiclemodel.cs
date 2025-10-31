using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Vehiclemodel
{
    public int Modelid { get; set; }

    public string Name { get; set; } = null!;

    public string? Brand { get; set; }

    public string? Enginetype { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<SparepartTuht> SparepartTuhts { get; set; } = new List<SparepartTuht>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
