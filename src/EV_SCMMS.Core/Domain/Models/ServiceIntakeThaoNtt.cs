namespace EV_SCMMS.Core.Domain.Models;

public partial class ServiceIntakeThaoNtt
{
    public Guid SithaoNttid { get; set; }

    public Guid BookingId { get; set; }

    public Guid? AdvisorId { get; set; }

    public DateTime? CheckinTimeUtc { get; set; }

    public int? OdometerKm { get; set; }

    public int? BatterySoC { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? Advisor { get; set; }

    public virtual ICollection<ChecklistResponseThaoNtt> ChecklistResponseThaoNtts { get; set; } = new List<ChecklistResponseThaoNtt>();
}
