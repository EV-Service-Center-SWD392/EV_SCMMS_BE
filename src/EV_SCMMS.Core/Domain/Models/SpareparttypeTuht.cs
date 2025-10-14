using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class SparePartTypeTuHt
{
    public Guid TypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<SparePartTuHt> SparePartTuHts { get; set; } = new List<SparePartTuHt>();
}
