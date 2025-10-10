using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Center
{
    public Guid Centerid { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<InventoryTuht> InventoryTuhts { get; set; } = new List<InventoryTuht>();

    public virtual ICollection<SparepartforecastTuht> SparepartforecastTuhts { get; set; } = new List<SparepartforecastTuht>();

    public virtual ICollection<Sparepartreplenishmentrequest> Sparepartreplenishmentrequests { get; set; } = new List<Sparepartreplenishmentrequest>();

    public virtual ICollection<SparepartusagehistoryTuht> SparepartusagehistoryTuhts { get; set; } = new List<SparepartusagehistoryTuht>();
}
