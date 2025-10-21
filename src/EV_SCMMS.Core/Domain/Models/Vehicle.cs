using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Vehicle
{
    public Guid Vehicleid { get; set; }

    public Guid Customerid { get; set; }

    public int? Modelid { get; set; }

    public string Licenseplate { get; set; } = null!;

    public int? Year { get; set; }

    public string? Color { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Bookinghuykt> Bookinghuykts { get; set; } = new List<Bookinghuykt>();

    public virtual Useraccount Customer { get; set; } = null!;

    public virtual ICollection<Maintenancehistorydungvm> Maintenancehistorydungvms { get; set; } = new List<Maintenancehistorydungvm>();

    public virtual Vehiclemodel? Model { get; set; }

    public virtual ICollection<Orderthaontt> Orderthaontts { get; set; } = new List<Orderthaontt>();

    public virtual ICollection<Vehicleconditiondungvm> Vehicleconditiondungvms { get; set; } = new List<Vehicleconditiondungvm>();
}
