using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class ActiveRefreshToken
{
    public Guid? Tokenid { get; set; }

    public Guid? Userid { get; set; }

    public string? Token { get; set; }

    public DateTime? Expiresat { get; set; }

    public DateTime? Createdat { get; set; }
}
