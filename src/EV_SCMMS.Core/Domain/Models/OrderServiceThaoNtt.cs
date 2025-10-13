namespace EV_SCMMS.Core.Domain.Models;

public partial class OrderServiceThaoNtt
{
    public int OsthaoNttid { get; set; }

    public int OrderId { get; set; }

    public int? ServiceId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual OrderThaoNtt Order { get; set; } = null!;

    public virtual Service? Service { get; set; }
}
