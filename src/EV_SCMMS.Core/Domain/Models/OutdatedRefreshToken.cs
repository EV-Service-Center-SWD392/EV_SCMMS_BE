using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class OutdatedRefreshToken
{
    public Guid Tokenid { get; set; }

    public Guid Userid { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Revokedat { get; set; }

    public string? Revokedreason { get; set; }

    public DateTime Expiresat { get; set; }

    public DateTime Createdat { get; set; }
}
