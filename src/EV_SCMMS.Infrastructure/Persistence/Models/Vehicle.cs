using System;
using System.Collections.Generic;

namespace EV_SCMMS.Infrastructure.Models;

public partial class Vehicle
{
    public Guid VehicleId { get; set; }

    public Guid? CustomerId { get; set; }

    public string? Vin { get; set; }

    public string? PlateNo { get; set; }

    public string? Model { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingThaoNtt> BookingThaoNtts { get; set; } = new List<BookingThaoNtt>();

    public virtual User? Customer { get; set; }

    public virtual ICollection<OrderThaoNtt> OrderThaoNtts { get; set; } = new List<OrderThaoNtt>();
}
