public class BookingQueryDto
{
  public int Page { get; set; } = 1;

  public int PageSize { get; set; } = 10;

  public Guid? CenterId { get; set; }

  public Guid? VehicleId { get; set; }

  public string? Status { get; set; }

  public string? DayOfWeek { get; set; }

  public DateTime? FromDate { get; set; }

  public DateTime? ToDate { get; set; }
}
