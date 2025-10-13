namespace EV_SCMMS.Core.Domain.Models;

public partial class OrderThaoNtt
{
    public int OrderThaoNttid { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? VehicleId { get; set; }

    public Guid? BookingId { get; set; }

    public short Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? PaymentId { get; set; }

    public virtual BookingThaoNtt? Booking { get; set; }

    public virtual User? Customer { get; set; }

    public virtual ICollection<OrderServiceThaoNtt> OrderServiceThaoNtts { get; set; } = new List<OrderServiceThaoNtt>();

    public virtual Vehicle? Vehicle { get; set; }

    public virtual WorkOrderApprovalThaoNtt? WorkOrderApprovalThaoNtt { get; set; }
}
