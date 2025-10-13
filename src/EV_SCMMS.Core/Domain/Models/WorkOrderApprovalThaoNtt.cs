namespace EV_SCMMS.Core.Domain.Models;

public partial class WorkOrderApprovalThaoNtt
{
    public Guid WoathaoNttid { get; set; }

    public int OrderId { get; set; }

    public short Status { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? Method { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual OrderThaoNtt Order { get; set; } = null!;
}
