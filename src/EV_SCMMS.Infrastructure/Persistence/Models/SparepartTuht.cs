using System;
using System.Collections.Generic;

namespace EV_SCMMS.Infrastructure.Models;

public partial class SparePartTuHt
{
    public Guid SparePartId { get; set; }

    public int? VehicleModelId { get; set; }

    public Guid? InventoryId { get; set; }

    public Guid? TypeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal? UnitPrice { get; set; }

    public string? Manufacture { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual InventoryTuHt? Inventory { get; set; }

    public virtual ICollection<SparePartForecastTuHt> SparePartForecastTuHts { get; set; } = new List<SparePartForecastTuHt>();

    public virtual ICollection<SparePartReplenishmentRequest> SparePartReplenishmentRequests { get; set; } = new List<SparePartReplenishmentRequest>();

    public virtual ICollection<SparePartUsageHistoryTuHt> SparePartUsageHistoryTuHts { get; set; } = new List<SparePartUsageHistoryTuHt>();

    public virtual SparePartTypeTuHt? Type { get; set; }
}
