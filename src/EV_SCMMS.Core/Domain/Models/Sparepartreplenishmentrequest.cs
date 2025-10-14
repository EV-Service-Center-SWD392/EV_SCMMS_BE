using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class SparePartReplenishmentRequest
{
    public Guid RequestId { get; set; }

    public Guid CenterId { get; set; }

    public Guid SparePartId { get; set; }

    public Guid? ForecastId { get; set; }

    public int? SuggestedQuantity { get; set; }

    public string? Status { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Center Center { get; set; } = null!;

    public virtual SparePartForecastTuHt? Forecast { get; set; }

    public virtual SparePartTuHt SparePart { get; set; } = null!;
}
