using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Receiptcuongtq
{
    public Guid Receiptid { get; set; }

    public Guid Orderid { get; set; }

    public string? Paymentmethod { get; set; }

    public decimal Totalamount { get; set; }

    public Guid Customerid { get; set; }

    public Guid Staffid { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Useraccount Customer { get; set; } = null!;

    public virtual Orderthaontt Order { get; set; } = null!;

    public virtual ICollection<Receiptitemcuongtq> Receiptitemcuongtqs { get; set; } = new List<Receiptitemcuongtq>();

    public virtual Useraccount Staff { get; set; } = null!;
}
