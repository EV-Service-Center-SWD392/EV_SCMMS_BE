using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Bookingschedule
{
    public Guid Slotid { get; set; }

    public Guid Centerid { get; set; }

    public DateTime Startutc { get; set; }

    public DateTime Endutc { get; set; }

    public int? Capacity { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Bookinghuykt> Bookinghuykts { get; set; } = new List<Bookinghuykt>();

    public virtual Centertuantm Center { get; set; } = null!;
}
