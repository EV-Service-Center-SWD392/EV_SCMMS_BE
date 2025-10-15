using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class RevokedToken
{
    public string? Token { get; set; }

    public Guid? Userid { get; set; }

    public DateTime? Revokedat { get; set; }

    public string? Revokedreason { get; set; }
}
