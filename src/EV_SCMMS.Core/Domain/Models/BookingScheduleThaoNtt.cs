using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class BookingScheduleThaoNtt
{
    public Guid BsthaoNttid { get; set; }

    public Guid CenterId { get; set; }

    public DateTime StartUtc { get; set; }

    public DateTime EndUtc { get; set; }

    public int Capacity { get; set; }

    public short Status { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingThaoNtt> BookingThaoNtts { get; set; } = new List<BookingThaoNtt>();

    public virtual Center Center { get; set; } = null!;
}
