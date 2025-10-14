using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class SparePartForecastTuHt
{
    public Guid ForecastId { get; set; }

    public Guid SparePartId { get; set; }

    public Guid CenterId { get; set; }

    public int? PredictedUsage { get; set; }

    public int? SafetyStock { get; set; }

    public int? ReorderPoint { get; set; }

    public string? ForecastedBy { get; set; }

    public decimal? ForecastConfidence { get; set; }

    public DateTime? ForecastDate { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Center Center { get; set; } = null!;

    public virtual SparePartTuHt SparePart { get; set; } = null!;

    public virtual ICollection<SparePartReplenishmentRequest> SparePartReplenishmentRequests { get; set; } = new List<SparePartReplenishmentRequest>();
}
