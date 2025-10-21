using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Receiptitemcuongtq
{
    public Guid Receiptitemid { get; set; }

    public Guid Receiptid { get; set; }

    public string Itemtype { get; set; } = null!;

    public string Itemname { get; set; } = null!;

    public string? Itemcode { get; set; }

    public int Quantity { get; set; }

    public decimal Unitprice { get; set; }

    public decimal Linetotal { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Receiptcuongtq Receipt { get; set; } = null!;
}
