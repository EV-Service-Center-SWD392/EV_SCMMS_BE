using EV_SCMMS.Core.Domain.Models;

public static class BookingschedulesExtensions
{
  public static List<Bookingschedule> ToEntities(this BookingScheduleDto dto)
  {
    return dto.CenterSchedules.Select(s => new Bookingschedule
    {
      Slotid = new Guid(),
      Centerid = dto.CenterId,
      Startutc = s.Start,
      Endutc = s.End,
      Capacity = s.Capacity,
      Note = s.Note,
      Status = "ACTIVE",
      Isactive = s.IsActive ?? true,
      DayOfWeek = s.DayOfWeek,
      Slot = s.Slot,
    }).ToList();
  }
}
