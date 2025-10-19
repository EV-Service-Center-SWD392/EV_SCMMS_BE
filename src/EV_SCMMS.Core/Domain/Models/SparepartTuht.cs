using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class SparepartTuht
{
    public Guid Sparepartid { get; set; }

    public int? Vehiclemodelid { get; set; }

    public Guid? Inventoryid { get; set; }

    public Guid? Typeid { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Unitprice { get; set; }

    public string? Manufacture { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual InventoryTuht? Inventory { get; set; }

    public virtual ICollection<SparepartforecastTuht> SparepartforecastTuhts { get; set; } = new List<SparepartforecastTuht>();

    public virtual ICollection<Sparepartreplenishmentrequest> Sparepartreplenishmentrequests { get; set; } = new List<Sparepartreplenishmentrequest>();

    public virtual ICollection<SparepartusagehistoryTuht> SparepartusagehistoryTuhts { get; set; } = new List<SparepartusagehistoryTuht>();

    public virtual SpareparttypeTuht? Type { get; set; }
}
