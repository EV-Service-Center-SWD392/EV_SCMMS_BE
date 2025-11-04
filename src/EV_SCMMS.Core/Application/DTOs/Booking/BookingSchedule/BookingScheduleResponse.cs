public class CenterSchedulesResponse
{
  public Guid CenterId { get; set; }
  public string CenterName { get; set; } = default!;
  public List<DayScheduleDto> Schedules { get; set; } = new();
}

public class CenterSchedulesResponseShorten
{
  public Guid CenterId { get; set; }
  public string CenterName { get; set; } = default!;
  public List<DayScheduleDtoShorten> Schedules { get; set; } = new();
}

public class DayScheduleDto : DayScheduleDtoShorten
{
  public DateOnly CurrentDate { get; set; } = default!;
}

public class DayScheduleDtoShorten
{
  public string DayOfWeek { get; set; } = default!;
  public List<BookingScheduleItemDto> Slots { get; set; } = new();
}

public class BookingScheduleItemDto
{
  public int Slot { get; set; }
  public string Startutc { get; set; } = default!;
  public string Endutc { get; set; } = default!;
  public int? Capacity { get; set; }
  public string? Note { get; set; }
  public string? Status { get; set; }
  public bool? IsActive { get; set; }

  public bool IsBookable { get; set; }

  public int CurrentBookingRecordCount { get; set; }
}
