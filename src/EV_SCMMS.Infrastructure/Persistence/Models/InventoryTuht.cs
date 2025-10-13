using System;
using System.Collections.Generic;

namespace EV_SCMMS.Infrastructure.Models;

public partial class InventoryTuHt
{
    public Guid InventoryId { get; set; }

    public Guid CenterId { get; set; }

    public int? Quantity { get; set; }

    public int? MinimumStockLevel { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Center Center { get; set; } = null!;

    public virtual ICollection<SparePartTuHt> SparePartTuHts { get; set; } = new List<SparePartTuHt>();
}
