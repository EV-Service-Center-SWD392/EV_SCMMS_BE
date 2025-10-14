using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Center
{
    public Guid CenterId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<InventoryTuHt> InventoryTuHts { get; set; } = new List<InventoryTuHt>();

    public virtual ICollection<SparePartForecastTuHt> SparePartForecastTuHts { get; set; } = new List<SparePartForecastTuHt>();

    public virtual ICollection<SparePartReplenishmentRequest> SparePartReplenishmentRequests { get; set; } = new List<SparePartReplenishmentRequest>();

    public virtual ICollection<SparePartUsageHistoryTuHt> SparePartUsageHistoryTuHts { get; set; } = new List<SparePartUsageHistoryTuHt>();
}
