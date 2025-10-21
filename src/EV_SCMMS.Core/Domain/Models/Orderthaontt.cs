using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Orderthaontt
{
    public Guid Orderid { get; set; }

    public Guid Customerid { get; set; }

    public Guid Vehicleid { get; set; }

    public Guid? Bookingid { get; set; }

    public int? Paymentid { get; set; }

    public decimal? Totalamount { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Bookinghuykt? Booking { get; set; }

    public virtual Useraccount Customer { get; set; } = null!;

    public virtual ICollection<Maintenancehistorydungvm> Maintenancehistorydungvms { get; set; } = new List<Maintenancehistorydungvm>();

    public virtual ICollection<Orderservicethaontt> Orderservicethaontts { get; set; } = new List<Orderservicethaontt>();

    public virtual ICollection<Ordersparepart> Orderspareparts { get; set; } = new List<Ordersparepart>();

    public virtual Paymentcuongtq? Payment { get; set; }

    public virtual Receiptcuongtq? Receiptcuongtq { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;

    public virtual ICollection<Workorderapprovalthaontt> Workorderapprovalthaontts { get; set; } = new List<Workorderapprovalthaontt>();
}
