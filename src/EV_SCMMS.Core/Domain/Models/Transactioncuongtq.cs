using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;


public partial class Transactioncuongtq
{
    public Guid Transactionid { get; set; }

    public Guid Orderid { get; set; }

    public int? Paymentmethodid { get; set; }

    public string? Description { get; set; }

    public string? Reason { get; set; }

    public decimal Totalamount { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public int Paymentid { get; set; }

    public string? Paymentlink { get; set; }

  public string? Status { get; set; } = "CREATED";
  public Guid? Staffid { get; set; }

    public virtual Orderthaontt Order { get; set; } = null!;

    public virtual Paymentmethodcuongtq? Paymentmethod { get; set; }

    public virtual ICollection<Receiptcuongtq> Receiptcuongtqs { get; set; } = new List<Receiptcuongtq>();

    public virtual Useraccount Staff { get; set; } = null!;
}
