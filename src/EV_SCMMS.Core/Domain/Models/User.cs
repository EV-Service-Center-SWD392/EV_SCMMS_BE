using EV_SCMMS.Core.Domain.Models;
using System;
using System.Collections.Generic;

namespace EV_SCMMS.Core.Domain.Models;

public partial class User
{
  public Guid Userid { get; set; }

  public string Email { get; set; } = null!;

  public string Password { get; set; } = null!;

  public string? Lastname { get; set; }

  public string? Firstname { get; set; }

  public DateOnly? Birthday { get; set; }

  public string? Address { get; set; }

  public string? Phonenumber { get; set; }

  public Guid Roleid { get; set; }

  public string? Status { get; set; }

  public bool? Isactive { get; set; }

  public DateTime? Createdat { get; set; }

  public DateTime? Updatedat { get; set; }

  public virtual Role Role { get; set; } = null!;

}
