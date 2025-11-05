public class BookingDetailDto
{
  public Guid BookingId { get; set; }
  public string Status { get; set; } = null!;
  public string? Notes { get; set; }
  public DateOnly BookingDate { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public bool? IsActive { get; set; }

  public BookingCustomerDto Customer { get; set; } = null!;
  public BookingVehicleDto Vehicle { get; set; } = null!;
  public BookingOrderDto? Order { get; set; }
  public BookingServiceIntakeDto? ServiceIntake { get; set; }
  public BookingSlotDto? Slot { get; set; }

  public List<BookingStatusLogDto> BookingStatusLogs { get; set; } = new();
}

public class BookingCustomerDto
{
  public Guid UserId { get; set; }
  public string Firstname { get; set; } = null!;
  public string Lastname { get; set; } = null!;
  public string Email { get; set; } = null!;
  public string Phonenumber { get; set; } = null!;
  public string Address { get; set; } = null!;
}

public class BookingVehicleDto
{
  public Guid VehicleId { get; set; }
  public string Licenseplate { get; set; } = null!;
  public string? Color { get; set; }
  public int? Year { get; set; }
  public string? Status { get; set; }
}

public class BookingOrderDto
{
  public Guid OrderId { get; set; }
  public string? Status { get; set; }
  public decimal? TotalAmount { get; set; }
  public DateTime? CreatedAt { get; set; }
}

public class BookingServiceIntakeDto
{
  public Guid IntakeId { get; set; }
  public string? Status { get; set; }
  public DateTime? CreatedAt { get; set; }
}

public class BookingSlotDto
{
  public Guid SlotId { get; set; }
  public string DayOfWeek { get; set; } = null!;
  public int Slot { get; set; }
  public string StartUtc { get; set; } = null!;
  public string EndUtc { get; set; } = null!;
  public string? Note { get; set; }

  public BookingCenterDto? Center { get; set; }
}

public class BookingCenterDto
{
  public Guid CenterId { get; set; }
  public string Name { get; set; } = null!;
  public string? Address { get; set; }
}

public class BookingStatusLogDto
{
  public Guid LogId { get; set; }
  public string Status { get; set; } = null!;
  public bool IsSeen { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
