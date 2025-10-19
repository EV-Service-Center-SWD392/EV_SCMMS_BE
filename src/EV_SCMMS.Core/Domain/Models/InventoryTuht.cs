using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class InventoryTuht
{
    public Guid Inventoryid { get; set; }

    public Guid Centerid { get; set; }

    public int? Quantity { get; set; }

    public int? Minimumstocklevel { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Center Center { get; set; } = null!;

    public virtual ICollection<SparepartTuht> SparepartTuhts { get; set; } = new List<SparepartTuht>();
}
