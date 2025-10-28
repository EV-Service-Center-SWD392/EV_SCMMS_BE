using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Workorderapprovalthaontt
{
    public Guid WoaId { get; set; }

    public Guid Orderid { get; set; }

    public string? Status { get; set; }

    public Guid? Approvedby { get; set; }

    public DateTime? Approvedat { get; set; }

    public string? Method { get; set; }

    public string? Note { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Orderthaontt? Order { get; set; } = null!;
}
