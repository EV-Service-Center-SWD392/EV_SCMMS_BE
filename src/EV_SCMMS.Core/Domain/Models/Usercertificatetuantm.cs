using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Usercertificatetuantm
{
    public Guid Usercertificateid { get; set; }

    public Guid Userid { get; set; }

    public Guid Certificateid { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Certificatetuantm Certificate { get; set; } = null!;

    public virtual Useraccount User { get; set; } = null!;
}
