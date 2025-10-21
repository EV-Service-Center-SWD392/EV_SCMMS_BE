using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Checklistresponsethaontt
{
    public Guid Responseid { get; set; }

    public Guid Intakeid { get; set; }

    public Guid Itemid { get; set; }

    public bool? Valuebool { get; set; }

    public decimal? Valuenumber { get; set; }

    public string? Valuetext { get; set; }

    public short? Severity { get; set; }

    public string? Comment { get; set; }

    public string? Photourl { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Serviceintakethaontt Intake { get; set; } = null!;

    public virtual Checklistitemthaontt Item { get; set; } = null!;
}
