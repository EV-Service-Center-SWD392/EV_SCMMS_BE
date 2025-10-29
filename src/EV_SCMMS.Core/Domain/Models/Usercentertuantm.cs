using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Usercentertuantm
{
    public Guid Usercenterid { get; set; }

    public Guid Userid { get; set; }

    public Guid Centerid { get; set; }

    public DateTime? Worksince { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Centertuantm Center { get; set; } = null!;

    public virtual Useraccount User { get; set; } = null!;
}
