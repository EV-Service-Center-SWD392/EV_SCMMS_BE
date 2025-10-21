using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Userworkscheduletuantm
{
    public Guid Userworkscheduleid { get; set; }

    public Guid Userid { get; set; }

    public Guid Workscheduleid { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Useraccount User { get; set; } = null!;

    public virtual Workscheduletuantm Workschedule { get; set; } = null!;
}
