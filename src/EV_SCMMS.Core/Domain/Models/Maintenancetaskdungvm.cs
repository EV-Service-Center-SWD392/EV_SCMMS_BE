using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Maintenancetaskdungvm
{
    public Guid Taskid { get; set; }

    public Guid Orderdetailid { get; set; }

    public Guid? Technicianid { get; set; }

    public string? Description { get; set; }

    public string? Task { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Orderservicethaontt Orderdetail { get; set; } = null!;

    public virtual Useraccount? Technician { get; set; }
}
