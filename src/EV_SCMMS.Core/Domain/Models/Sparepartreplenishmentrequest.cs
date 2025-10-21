using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Sparepartreplenishmentrequest
{
    public Guid Requestid { get; set; }

    public Guid Centerid { get; set; }

    public Guid Sparepartid { get; set; }

    public Guid? Forecastid { get; set; }

    public int? Suggestedquantity { get; set; }

    public Guid? Approvedby { get; set; }

    public DateTime? Approvedat { get; set; }

    public string? Notes { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Centertuantm Center { get; set; } = null!;

    public virtual SparepartforecastTuht? Forecast { get; set; }

    public virtual SparepartTuht Sparepart { get; set; } = null!;
}
