using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

public class BookingScheduleService
{
  private readonly IUnitOfWork _unitOfWork;

  public BookingScheduleService(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public static int NumberOfCurrentBookingRecord(Bookingschedule b, DateOnly bookingDate)
  {
    return b.Bookinghuykts.Where(x => x.BookingDate == bookingDate && BookingStatusConstant.ProcessingBookingRecord.Contains(x.Status) && x.Isactive == true).Count();
  }

  public async Task<CenterSchedulesResponseShorten?> GetCenterSchedulesAsync(Guid centerId)
  {
    var centerSchedules = await _unitOfWork.CenterRepository.GetAllQueryable()
        .AsNoTracking()
        .Where(c => c.Centerid == centerId)
        .Select(c => new CenterSchedulesResponseShorten
        {
          CenterId = c.Centerid,
          CenterName = c.Name,
          Schedules = c.Bookingschedules
                .GroupBy(s => s.DayOfWeek)
                .Select(g => new DayScheduleDtoShorten
                {
                  DayOfWeek = g.Key,
                  Slots = g
                        .OrderBy(s => s.Slot)
                        .Select(s => new BookingScheduleItemDto
                        {
                          Slot = s.Slot,
                          Startutc = s.Startutc,
                          Endutc = s.Endutc,
                          Capacity = s.Capacity,
                          Note = s.Note,
                          Status = s.Status,
                          IsActive = s.Isactive
                        })
                        .ToList()
                })
                .ToList()
        })
        .FirstOrDefaultAsync();

    return centerSchedules;
  }


  public async Task<IServiceResult<CenterSchedulesResponse>> GetBookingSchedulesByCenter(Guid centerId, DateOnly startDate, DateOnly endDate)
  {
    try
    {
      if (endDate < startDate)
        return ServiceResult<CenterSchedulesResponse>.Failure("End date must be after or equal to start date");

      // Load center with all schedules and bookings (to compute per day counts)
      var center = await _unitOfWork.CenterRepository.GetAllQueryable()
          .Include(x => x.Bookingschedules)  // All schedules for center
          .ThenInclude(x => x.Bookinghuykts)  // All bookings for schedules
          .FirstOrDefaultAsync(x => x.Centerid == centerId);

      if (center == null)
        return ServiceResult<CenterSchedulesResponse>.Failure("Center is not found");

      var daysDifference = (endDate.Year - startDate.Year) * 365 + (endDate.DayOfYear - startDate.DayOfYear);

      var dateRange = Enumerable.Range(0, daysDifference + 1)
          .Select(offset => startDate.AddDays(offset))
          .ToList();

      // Group schedules by DayOfWeek (recurring weekly)
      var schedulesByDayOfWeek = center.Bookingschedules
          .Where(s => s.Isactive == true)  // Only active schedules
          .GroupBy(s => s.DayOfWeek)
          .ToDictionary(g => g.Key, g => g.ToList());

      // Build response: For each date in range, map schedules for that DayOfWeek
      var dailySchedules = dateRange.Select(currentDate =>
      {
        var dayOfWeekValue = GetDayOfWeekValue(currentDate.DayOfWeek);  // Convert DayOfWeek enum to "Mon", etc.

        if (!schedulesByDayOfWeek.ContainsKey(dayOfWeekValue))
          return new DayScheduleDto  // No schedules for this day
          {
            DayOfWeek = dayOfWeekValue,
            CurrentDate = currentDate,
            Slots = new List<BookingScheduleItemDto>()
          };

        var daySchedules = schedulesByDayOfWeek[dayOfWeekValue];

        return new DayScheduleDto
        {
          DayOfWeek = dayOfWeekValue,
          CurrentDate = currentDate,
          Slots = daySchedules.Select(s => new BookingScheduleItemDto
          {
            Slot = s.Slot,
            Startutc = s.Startutc,
            Endutc = s.Endutc,
            Capacity = s.Capacity,
            Note = s.Note,
            Status = s.Status,
            IsActive = s.Isactive,
            CurrentBookingRecordCount = s.Bookinghuykts.Count(bh =>
                  bh.BookingDate == currentDate  // Filter by specific date
                  && BookingStatusConstant.ProcessingBookingRecord.Contains(bh.Status)
                  && bh.Isactive == true),  // Active + processing status
            IsBookable = s.Bookinghuykts.Count(bh =>
                  bh.BookingDate == currentDate
                  && BookingStatusConstant.ProcessingBookingRecord.Contains(bh.Status)
                  && bh.Isactive == true) < (s.Capacity ?? int.MaxValue)  // Compare with capacity
          })
              .OrderBy(s => s.Slot)  // Order by slot number
              .ToList()
        };
      }).ToList();

      return ServiceResult<CenterSchedulesResponse>.Success(new CenterSchedulesResponse
      {
        CenterId = center.Centerid,
        CenterName = center.Name,
        Schedules = dailySchedules  // List of daily schedules in range
      }, "Get schedules for date range successfully");
    }
    catch (Exception ex)
    {
      // Log error
      Console.WriteLine($"Error in GetBookingSchedulesByCenter: {ex.Message}");
      return ServiceResult<CenterSchedulesResponse>.Failure("Something went wrong");
    }
  }

  public static string GetDayOfWeekValue(DayOfWeek dayOfWeek)
  {
    return dayOfWeek switch
    {
      DayOfWeek.Monday => DayOfWeekConstant.Mon,
      DayOfWeek.Tuesday => DayOfWeekConstant.Tue,
      DayOfWeek.Wednesday => DayOfWeekConstant.Wed,
      DayOfWeek.Thursday => DayOfWeekConstant.Thu,
      DayOfWeek.Friday => DayOfWeekConstant.Fri,
      DayOfWeek.Saturday => DayOfWeekConstant.Sat,
      DayOfWeek.Sunday => DayOfWeekConstant.Sun,
      _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek))
    };
  }

  public async Task<IServiceResult> UpsertBookingSchedulesByCenterAsync(BookingScheduleDto dto)
  {
    var center = await _unitOfWork.CenterRepository.GetAllQueryable()
        .Include(c => c.Bookingschedules)
        .FirstOrDefaultAsync(c => c.Centerid == dto.CenterId);

    if (center == null)
      return ServiceResult.Failure("Center not found");

    if (center.Bookingschedules.Any())
    {
      await _unitOfWork.BookingScheduleRepository.RemoveMultipleEntitiesAsync(center.Bookingschedules.ToList());
    }

    var newSchedules = dto.CenterSchedules.Select(s => new Bookingschedule
    {
      Slotid = Guid.NewGuid(),                // generate mới
      Centerid = dto.CenterId,
      Startutc = s.Start,
      Endutc = s.End,
      Capacity = s.Capacity,
      Note = s.Note,
      Status = "ACTIVE",
      Isactive = s.IsActive ?? true,
      DayOfWeek = s.DayOfWeek,
      Slot = s.Slot,
      Createdat = DateTime.UtcNow,
      Updatedat = DateTime.UtcNow
    }).ToList();

    await _unitOfWork.BookingScheduleRepository.AddMultipleAsync(newSchedules);
    await _unitOfWork.SaveChangesAsync();

    return ServiceResult.Success("Booking schedules updated successfully");
  }


}
