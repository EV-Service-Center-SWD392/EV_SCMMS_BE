using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Orderservicethaontt
{
    public Guid Orderdetailid { get; set; }

    public Guid Orderid { get; set; }

    public Guid Serviceid { get; set; }

    public int? Quantity { get; set; }

    public decimal? Unitprice { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Maintenancetaskdungvm> Maintenancetaskdungvms { get; set; } = new List<Maintenancetaskdungvm>();

    public virtual Orderthaontt Order { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
