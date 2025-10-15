using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class AssignmentThaoNtt
{
    public Guid AssThaoNttid { get; set; }

    public Guid BookingId { get; set; }

    public Guid TechnicianId { get; set; }

    public DateTime? PlannedStartUtc { get; set; }

    public DateTime? PlannedEndUtc { get; set; }

    public string? Status { get; set; }

    public int? QueueNo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual BookingThaoNtt Booking { get; set; } = null!;

    public virtual User Technician { get; set; } = null!;
}
