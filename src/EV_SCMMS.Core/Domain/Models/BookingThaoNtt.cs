namespace EV_SCMMS.Core.Domain.Models;

public partial class BookingThaoNtt
{
    public Guid BookingThaoNttid { get; set; }

    public Guid CustomerId { get; set; }

    public Guid VehicleId { get; set; }

    public Guid? BookingScheduleId { get; set; }

    public short Status { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AssignmentThaoNtt> AssignmentThaoNtts { get; set; } = new List<AssignmentThaoNtt>();

    public virtual BookingScheduleThaoNtt? BookingSchedule { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<OrderThaoNtt> OrderThaoNtts { get; set; } = new List<OrderThaoNtt>();

    public virtual ICollection<ServiceIntakeThaoNtt> ServiceIntakeThaoNtts { get; set; } = new List<ServiceIntakeThaoNtt>();

    public virtual Vehicle Vehicle { get; set; } = null!;
}
