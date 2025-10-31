using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Ordersparepart
{
    public Guid Ordersparepartid { get; set; }

    public Guid Orderid { get; set; }

    public Guid Sparepartid { get; set; }

    public int Quantity { get; set; }

    public decimal? Unitprice { get; set; }

    public decimal? Discount { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Orderthaontt Order { get; set; } = null!;

    public virtual SparepartTuht Sparepart { get; set; } = null!;

    public virtual ICollection<SparepartusagehistoryTuht> SparepartusagehistoryTuhts { get; set; } = new List<SparepartusagehistoryTuht>();
}
