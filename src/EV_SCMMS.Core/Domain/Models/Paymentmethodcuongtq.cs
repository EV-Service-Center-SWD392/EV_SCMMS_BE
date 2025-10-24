using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;


public partial class Paymentmethodcuongtq
{
    public int Paymentmethodid { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Transactioncuongtq> Transactioncuongtqs { get; set; } = new List<Transactioncuongtq>();
}
