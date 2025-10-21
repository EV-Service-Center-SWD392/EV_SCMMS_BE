using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Checklistitemthaontt
{
    public Guid Itemid { get; set; }

    public string? Code { get; set; }

    public string Name { get; set; } = null!;

    public short? Type { get; set; }

    public string? Unit { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Checklistresponsethaontt> Checklistresponsethaontts { get; set; } = new List<Checklistresponsethaontt>();
}
