using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;


public partial class Receiptitemcuongtq
{
    public Guid Receiptitemid { get; set; }

    public Guid Receiptid { get; set; }

    public string Itemname { get; set; } = null!;

    public Guid? Itemid { get; set; }

    public int Quantity { get; set; }

    public decimal Unitprice { get; set; }

    public decimal Linetotal { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Receiptcuongtq Receipt { get; set; } = null!;
}
