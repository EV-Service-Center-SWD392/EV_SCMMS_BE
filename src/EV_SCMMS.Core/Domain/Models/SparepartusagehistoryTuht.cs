using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class SparepartusagehistoryTuht
{
    public Guid Usageid { get; set; }

    public Guid Sparepartid { get; set; }

    public Guid Centerid { get; set; }

    public Guid? Ordersparepartid { get; set; }

    public int Quantityused { get; set; }

    public DateTime? Useddate { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Centertuantm Center { get; set; } = null!;

    public virtual Ordersparepart? Ordersparepart { get; set; }

    public virtual SparepartTuht Sparepart { get; set; } = null!;
}
