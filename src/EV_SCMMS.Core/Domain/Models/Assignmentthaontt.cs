using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Assignmentthaontt
{
    public Guid Assignmentid { get; set; }

    public Guid Bookingid { get; set; }

    public Guid Technicianid { get; set; }

    public DateTime? Plannedstartutc { get; set; }

    public DateTime? Plannedendutc { get; set; }

    public int? Queueno { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Bookinghuykt Booking { get; set; } = null!;

    public virtual Useraccount Technician { get; set; } = null!;
}
