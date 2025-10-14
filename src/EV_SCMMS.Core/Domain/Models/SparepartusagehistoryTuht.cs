using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class SparePartUsageHistoryTuHt
{
    public Guid UsageId { get; set; }

    public Guid SparePartId { get; set; }

    public Guid CenterId { get; set; }

    public int QuantityUsed { get; set; }

    public DateTime? UsedDate { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Center Center { get; set; } = null!;

    public virtual SparePartTuHt SparePart { get; set; } = null!;
}
