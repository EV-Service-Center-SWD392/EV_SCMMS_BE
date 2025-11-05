public class BookingScheduleDto
{
  public Guid CenterId { get; set; }

  public List<BookingSchedulesAttributesDto> CenterSchedules { get; set; } = new();
}

public class BookingSchedulesAttributesDto
{
  public string Start { get; set; } = default!;
  public string End { get; set; } = default!;

  public int? Capacity { get; set; }
  public string? Note { get; set; }

  public bool? IsActive { get; set; }

  public int Slot { get; set; }
  public string DayOfWeek { get; set; } = default!;
}
