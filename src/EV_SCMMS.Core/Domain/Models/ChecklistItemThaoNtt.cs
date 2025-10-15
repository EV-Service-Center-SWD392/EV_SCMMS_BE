using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class ChecklistItemThaoNtt
{
    public Guid ClithaoNttid { get; set; }

    public string? Code { get; set; }

    public string Name { get; set; } = null!;

    public short Type { get; set; }

    public string? Unit { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ChecklistResponseThaoNtt> ChecklistResponseThaoNtts { get; set; } = new List<ChecklistResponseThaoNtt>();
}
