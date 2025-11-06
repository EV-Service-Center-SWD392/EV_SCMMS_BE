public class CreateBookingDto
{
  public string BookingDate { get; set; }

  public int Slot { get; set; }
  public Guid VehicleId { get; set; }
  public string? Notes { get; set; }
  public Guid? CenterId { get; set; }
}

public class UpdateBookingDto : CreateBookingDto
{
  public bool IsCancel { get; set; }
}
