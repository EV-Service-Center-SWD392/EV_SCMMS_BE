using System;

namespace EV_SCMMS.Core.Domain.Models;

/// <summary>
/// Represents a technician work schedule per date and time slot
/// </summary>
public class WorkScheduleTuantm
{
    public Guid Id { get; set; }
    public Guid TechnicianId { get; set; }
    public DateOnly WorkDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotCapacity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

