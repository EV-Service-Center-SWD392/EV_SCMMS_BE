using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class Useraccount
{
    public Guid Userid { get; set; }

    public Guid Roleid { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phonenumber { get; set; }

    public string? Address { get; set; }

    public string? Lastname { get; set; }

    public string? Firstname { get; set; }

    public string? Status { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Assignmentthaontt> Assignmentthaontts { get; set; } = new List<Assignmentthaontt>();

    public virtual ICollection<Bookinghuykt> Bookinghuykts { get; set; } = new List<Bookinghuykt>();

    public virtual ICollection<Maintenancetaskdungvm> Maintenancetaskdungvms { get; set; } = new List<Maintenancetaskdungvm>();

    public virtual ICollection<Orderthaontt> Orderthaontts { get; set; } = new List<Orderthaontt>();

    public virtual ICollection<Receiptcuongtq> ReceiptcuongtqCustomers { get; set; } = new List<Receiptcuongtq>();

    public virtual ICollection<Receiptcuongtq> ReceiptcuongtqStaffs { get; set; } = new List<Receiptcuongtq>();

    public virtual ICollection<Refreshtoken> Refreshtokens { get; set; } = new List<Refreshtoken>();

    public virtual Userrole Role { get; set; } = null!;

    public virtual ICollection<Serviceintakethaontt> Serviceintakethaontts { get; set; } = new List<Serviceintakethaontt>();

    public virtual ICollection<Usercentertuantm> Usercentertuantms { get; set; } = new List<Usercentertuantm>();

    public virtual ICollection<Usercertificatetuantm> Usercertificatetuantms { get; set; } = new List<Usercertificatetuantm>();

    public virtual ICollection<Userworkscheduletuantm> Userworkscheduletuantms { get; set; } = new List<Userworkscheduletuantm>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
