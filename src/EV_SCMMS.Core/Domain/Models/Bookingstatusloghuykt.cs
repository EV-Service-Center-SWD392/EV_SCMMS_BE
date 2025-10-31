using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Bookingstatusloghuykt
{
    public Guid Logid { get; set; }

    public Guid Bookingid { get; set; }

    public string Status { get; set; } = null!;

    public bool? Isseen { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Bookinghuykt Booking { get; set; } = null!;
}
