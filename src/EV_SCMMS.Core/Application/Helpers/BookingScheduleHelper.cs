public class BookingScheduleHelper
{
  public static bool NoOverlap(List<BookingSchedulesAttributesDto> schedules)
  {
    var grouped = schedules.GroupBy(x => x.DayOfWeek);

    foreach (var group in grouped)
    {

      foreach (var timeData in group)
      {
        if (!IsValidTimeRange(timeData.Start, timeData.End))
        {
          return false;
        }
      }

      var ordered = group
          .Select(x =>
          {
            return new
            {
              Start = TimeSpan.Parse(x.Start),
              End = TimeSpan.Parse(x.End)
            };
          })
          .OrderBy(x => x.Start)
          .ToList();

      for (int i = 0; i < ordered.Count - 1; i++)
      {
        if (ordered[i].End > ordered[i + 1].Start)
          return false;
      }
    }
    return true;
  }

  public static bool IsValidTime(List<BookingSchedulesAttributesDto> schedules)
  {
    var grouped = schedules.GroupBy(x => x.DayOfWeek);

    foreach (var group in grouped)
    {
      foreach (var timeData in group)
      {
        if (!IsValidTimeRange(timeData.Start, timeData.End))
        {
          return false;
        }
      }
    }

    return true;
  }

  public static bool IsValidTimeRange(string start, string end)
  {
    return TimeSpan.TryParse(start, out var s)
        && TimeSpan.TryParse(end, out var e)
        && e > s;
  }
}
