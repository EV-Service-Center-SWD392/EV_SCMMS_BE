using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Workscheduletuantm
{
    public Guid Workscheduleid { get; set; }

    public Guid Centerid { get; set; }

    public DateTime Starttime { get; set; }

    public DateTime Endtime { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Centertuantm Center { get; set; } = null!;

    public virtual ICollection<Userworkscheduletuantm> Userworkscheduletuantms { get; set; } = new List<Userworkscheduletuantm>();
}
