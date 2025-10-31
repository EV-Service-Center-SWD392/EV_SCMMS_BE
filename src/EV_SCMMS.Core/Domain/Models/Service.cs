using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Service
{
    public Guid Serviceid { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Durationminutes { get; set; }

    public decimal? Baseprice { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Orderservicethaontt> Orderservicethaontts { get; set; } = new List<Orderservicethaontt>();
}
