using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Service
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? DurationMinutes { get; set; }

    public decimal? BasePrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OrderServiceThaoNtt> OrderServiceThaoNtts { get; set; } = new List<OrderServiceThaoNtt>();
}
