using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class SparepartforecastTuht
{
    public Guid Forecastid { get; set; }

    public Guid Sparepartid { get; set; }

    public Guid Centerid { get; set; }

    public int? Predictedusage { get; set; }

    public int? Safetystock { get; set; }

    public int? Reorderpoint { get; set; }

    public string? Forecastedby { get; set; }

    public decimal? Forecastconfidence { get; set; }

    public DateTime? Forecastdate { get; set; }

    public Guid? Approvedby { get; set; }

    public DateTime? Approveddate { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Centertuantm Center { get; set; } = null!;

    public virtual SparepartTuht Sparepart { get; set; } = null!;

    public virtual ICollection<Sparepartreplenishmentrequest> Sparepartreplenishmentrequests { get; set; } = new List<Sparepartreplenishmentrequest>();
}
