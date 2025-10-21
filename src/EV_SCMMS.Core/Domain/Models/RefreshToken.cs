using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Refreshtoken
{
    public Guid Tokenid { get; set; }

    public Guid Userid { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Expiresat { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual Useraccount User { get; set; } = null!;
}
