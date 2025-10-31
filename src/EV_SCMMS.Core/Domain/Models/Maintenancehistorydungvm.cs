using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Maintenancehistorydungvm
{
    public Guid Historyid { get; set; }

    public Guid Vehicleid { get; set; }

    public Guid? Orderid { get; set; }

    public DateTime Completeddate { get; set; }

    public string? Summary { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Orderthaontt? Order { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
