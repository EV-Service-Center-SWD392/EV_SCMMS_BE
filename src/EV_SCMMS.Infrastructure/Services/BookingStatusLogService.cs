using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class BookingStatusLogService
{
  private readonly ILogger<BookingStatusLogService> _logger;
  private readonly IUnitOfWork _unitOfWork;

  public BookingStatusLogService(IUnitOfWork unitOfWork, ILogger<BookingStatusLogService> logger)
  {
    _unitOfWork = unitOfWork;
    _logger = logger;
  }

  public async Task AddLogAsync(Guid bookingId)
  {
    var existed = await _unitOfWork.BookingRepository.GetAllQueryable().FirstOrDefaultAsync(b => b.Bookingid == bookingId);
    if (existed == null)
    {
      _logger.LogWarning("Tried to insert booking status log for non-existing booking {BookingId}", bookingId);
      return;
    }

    var log = new Bookingstatusloghuykt
    {
      Logid = Guid.NewGuid(),
      Bookingid = bookingId,
      Status = existed.Status,
      Isseen = false,
      Isactive = true,
      Createdat = DateTime.UtcNow,
      Updatedat = DateTime.UtcNow
    };

    await _unitOfWork.BookingStatusLogRepository.AddAsync(log);
    await _unitOfWork.SaveChangesAsync();

    _logger.LogInformation("Inserted booking status log for booking {BookingId} with status {Status}", bookingId, existed.Status);
  }

  public async Task<List<BookingStatusLogResponseDto>> GetLogsAsync(Guid customerId, Guid? bookingId)
  {
    try
    {
      var queryBuilder = _unitOfWork.BookingStatusLogRepository.GetAllQueryable()
        .AsNoTracking()
        .Include(x => x.Booking).AsNoTracking();

      Console.WriteLine($"bookingId: {bookingId} | Is Empty: {bookingId == Guid.Empty}");

      if (bookingId != Guid.Empty)
      {
        queryBuilder = queryBuilder.Where(x => x.Bookingid == bookingId);
      }


      var logs = await queryBuilder.Where(x => x.Booking.Customerid == customerId)
      .OrderByDescending(x => x.Updatedat)
      .Select(x => new BookingStatusLogResponseDto
      {
        LogId = x.Logid,
        BookingId = x.Bookingid,
        Status = x.Status,
        CreatedAt = x.Createdat,
        UpdatedAt = x.Updatedat,
        CustomerId = x.Booking.Customerid,
        VehicleId = x.Booking.Vehicleid,
        SlotId = x.Booking.Slotid,
        Notes = x.Booking.Notes,
        IsSeen = x.Isseen || false,
      })
      .ToListAsync();

      _logger.LogInformation("Fetched {Count} booking status logs for customer {CustomerId}", logs.Count, customerId);

      return logs;
    }
    catch (Exception e)
    {
      _logger.LogError("Get log asyncs error", e);
      return new List<BookingStatusLogResponseDto>();
    }
  }
}
