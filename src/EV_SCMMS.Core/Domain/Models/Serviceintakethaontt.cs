using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Serviceintakethaontt
{
    public Guid Intakeid { get; set; }

    public Guid Bookingid { get; set; }

    public Guid Advisorid { get; set; }

    public DateTime? Checkintimeutc { get; set; }

    public int? Odometerkm { get; set; }

    public int? Batterysoc { get; set; }

    public string? Notes { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Useraccount Advisor { get; set; } = null!;

    public virtual Bookinghuykt Booking { get; set; } = null!;

    public virtual ICollection<Checklistresponsethaontt> Checklistresponsethaontts { get; set; } = new List<Checklistresponsethaontt>();
}
