using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Bookinghuykt
{
    public Guid Bookingid { get; set; }

    public Guid Customerid { get; set; }

    public Guid Vehicleid { get; set; }

    public Guid? Slotid { get; set; }

    public string? Notes { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Assignmentthaontt> Assignmentthaontts { get; set; } = new List<Assignmentthaontt>();

    public virtual ICollection<Bookingstatusloghuykt> Bookingstatusloghuykts { get; set; } = new List<Bookingstatusloghuykt>();

    public virtual Useraccount Customer { get; set; } = null!;

    public virtual Orderthaontt? Orderthaontt { get; set; }

    public virtual Serviceintakethaontt? Serviceintakethaontt { get; set; }

    public virtual Bookingschedule? Slot { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
