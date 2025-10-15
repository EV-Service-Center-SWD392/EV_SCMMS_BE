using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class User
{
    public Guid UserCuongtqld { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? FullName { get; set; }

    public virtual ICollection<AssignmentThaoNtt> AssignmentThaoNtts { get; set; } = new List<AssignmentThaoNtt>();

    public virtual ICollection<BookingThaoNtt> BookingThaoNtts { get; set; } = new List<BookingThaoNtt>();

    public virtual ICollection<MaintenanceTaskDungVm> MaintenanceTaskDungVms { get; set; } = new List<MaintenanceTaskDungVm>();

    public virtual ICollection<OrderThaoNtt> OrderThaoNtts { get; set; } = new List<OrderThaoNtt>();

    public virtual ICollection<ServiceIntakeThaoNtt> ServiceIntakeThaoNtts { get; set; } = new List<ServiceIntakeThaoNtt>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
