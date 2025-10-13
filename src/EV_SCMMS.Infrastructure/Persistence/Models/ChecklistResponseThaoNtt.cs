using System;
using System.Collections.Generic;

namespace EV_SCMMS.Infrastructure.Models;

public partial class ChecklistResponseThaoNtt
{
    public Guid CrthaoNttid { get; set; }

    public Guid IntakeId { get; set; }

    public Guid ItemId { get; set; }

    public bool? ValueBool { get; set; }

    public decimal? ValueNumber { get; set; }

    public string? ValueText { get; set; }

    public short? Severity { get; set; }

    public string? Comment { get; set; }

    public string? PhotoUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ServiceIntakeThaoNtt Intake { get; set; } = null!;

    public virtual ChecklistItemThaoNtt Item { get; set; } = null!;
}
