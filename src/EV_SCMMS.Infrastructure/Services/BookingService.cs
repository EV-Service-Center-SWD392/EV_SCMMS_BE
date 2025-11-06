using EV_SCMMS.Core.Application.DTOs.Common;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
public class BookingService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly BookingStatusLogService _bookingStatusLogService;

  public BookingService(IUnitOfWork unitOfWork, BookingStatusLogService bookingStatusLogService)
  {
    _unitOfWork = unitOfWork;
    _bookingStatusLogService = bookingStatusLogService;
  }




  // Get all bookings (by center, by client, from time to time, by status)

  public async Task<IServiceResult<PagedResult<BookingWithSlotDto>>> GetBookingsAsync(Guid customerId, BookingQueryDto query, CancellationToken ct = default)
  {
    try
    {
      var queryable = _unitOfWork.BookingRepository.GetAllQueryable()
          .Include(b => b.Slot)
          .ThenInclude(s => s.Center)
          .AsNoTracking();


      queryable = queryable.Where(b => b.Customerid == customerId);

      if (query.VehicleId.HasValue)
        queryable = queryable.Where(b => b.Vehicleid == query.VehicleId.Value);

      if (query.CenterId.HasValue)
        queryable = queryable.Where(b => b.Slotid.HasValue && b.Slot.Centerid == query.CenterId.Value);

      if (!string.IsNullOrEmpty(query.DayOfWeek))
        queryable = queryable.Where(b => b.Slotid.HasValue && b.Slot.DayOfWeek.Equals(query.DayOfWeek, StringComparison.OrdinalIgnoreCase));

      if (!string.IsNullOrEmpty(query.Status))
        queryable = queryable.Where(b => b.Status.Equals(query.Status, StringComparison.OrdinalIgnoreCase));

      if (query.FromDate.HasValue)
        queryable = queryable.Where(b => b.Createdat >= query.FromDate.Value);

      if (query.ToDate.HasValue)
        queryable = queryable.Where(b => b.Createdat <= query.ToDate.Value);

      // Total count
      var totalCount = await queryable.CountAsync(ct);

      // Pagination + OrderBy (mới nhất trước)
      var items = await queryable
          .OrderByDescending(b => b.Createdat)
          .Skip((query.Page - 1) * query.PageSize)
          .Take(query.PageSize).Select(b => new BookingWithSlotDto
          {
            BookingId = b.Bookingid,
            CustomerId = b.Customerid,
            VehicleId = b.Vehicleid,
            SlotId = b.Slotid,
            Notes = b.Notes,
            Status = b.Status,
            IsActive = b.Isactive,
            BookingDate = b.BookingDate,
            CreatedAt = b.Createdat,
            UpdatedAt = b.Updatedat,

            Slot = b.Slot == null ? null : new BookingSlotDto
            {
              SlotId = b.Slot.Slotid,
              StartUtc = b.Slot.Startutc,
              EndUtc = b.Slot.Endutc,
              DayOfWeek = b.Slot.DayOfWeek,
              Slot = b.Slot.Slot,
              Note = b.Slot.Note,
              Center = new BookingCenterDto
              {
                CenterId = b.Slot.Center.Centerid,
                Name = b.Slot.Center.Name,
                Address = b.Slot.Center.Address
              }
            }
          })
          .ToListAsync(ct);



      return ServiceResult<PagedResult<BookingWithSlotDto>>.Success(new PagedResult<BookingWithSlotDto>
      {
        Items = items,
        Total = totalCount,
        Page = query.Page,
        PageSize = query.PageSize
      }, "Get bookings successfully");
    }
    catch (Exception ex)
    {
      // Log nếu có ILogger
      Console.WriteLine($"Error in GetBookingsAsync: {ex.Message}");
      return ServiceResult<PagedResult<BookingWithSlotDto>>.Failure("Something was wrong");
    }
  }


  // Get booking details (include user, order, booking status log, serviceintake, vehicle, center)

  public async Task<IServiceResult<BookingDetailDto>> GetBookingDetailsAsync(Guid customerId, Guid bookingId, CancellationToken ct = default)
  {
    var booking = await _unitOfWork.BookingRepository
        .GetAllQueryable()
        .AsNoTracking()
        .Include(b => b.Customer)
        .Include(b => b.Vehicle)
        .Include(b => b.Orderthaontt)
        .Include(b => b.Serviceintakethaontt)
        .Include(b => b.Bookingstatusloghuykts)
        .Include(b => b.Slot)
            .ThenInclude(s => s.Center)
        .Where(b => b.Bookingid == bookingId && b.Customerid == customerId)
        .Select(b => new BookingDetailDto
        {
          BookingId = b.Bookingid,
          Status = b.Status!,
          Notes = b.Notes,
          BookingDate = b.BookingDate,
          CreatedAt = b.Createdat,
          UpdatedAt = b.Updatedat,
          IsActive = b.Isactive,

          Customer = new BookingCustomerDto
          {
            UserId = b.Customer.Userid,
            Firstname = b.Customer.Firstname,
            Lastname = b.Customer.Lastname,
            Email = b.Customer.Email,
            Phonenumber = b.Customer.Phonenumber,
            Address = b.Customer.Address
          },

          Vehicle = new BookingVehicleDto
          {
            VehicleId = b.Vehicle.Vehicleid,
            Licenseplate = b.Vehicle.Licenseplate,
            Color = b.Vehicle.Color,
            Year = b.Vehicle.Year,
            Status = b.Vehicle.Status
          },

          Order = b.Orderthaontt == null ? null : new BookingOrderDto
          {
            OrderId = b.Orderthaontt.Orderid,
            Status = b.Orderthaontt.Status,
            TotalAmount = b.Orderthaontt.Totalamount,
            CreatedAt = b.Orderthaontt.Createdat
          },

          ServiceIntake = b.Serviceintakethaontt == null ? null : new BookingServiceIntakeDto
          {
            IntakeId = b.Serviceintakethaontt.Intakeid,
            Status = b.Serviceintakethaontt.Status,
            CreatedAt = b.Serviceintakethaontt.Createdat
          },

          Slot = b.Slot == null ? null : new BookingSlotDto
          {
            SlotId = b.Slot.Slotid,
            DayOfWeek = b.Slot.DayOfWeek,
            Slot = b.Slot.Slot,
            StartUtc = b.Slot.Startutc,
            EndUtc = b.Slot.Endutc,
            Note = b.Slot.Note,
            Center = b.Slot.Center == null ? null : new BookingCenterDto
            {
              CenterId = b.Slot.Center.Centerid,
              Name = b.Slot.Center.Name,
              Address = b.Slot.Center.Address
            }
          },

          BookingStatusLogs = b.Bookingstatusloghuykts
                .OrderByDescending(l => l.Updatedat)
                .Select(l => new BookingStatusLogDto
                {
                  LogId = l.Logid,
                  Status = l.Status,
                  IsSeen = l.Isseen,
                  CreatedAt = l.Createdat,
                  UpdatedAt = l.Updatedat
                })
                .ToList()
        })
        .FirstOrDefaultAsync(ct);

    if (booking == null)
      return ServiceResult<BookingDetailDto>.Failure("Booking not found or unauthorized");

    return ServiceResult<BookingDetailDto>.Success(booking, "Get booking details successfully");

  }

  public async Task<IServiceResult> UpdateBookingAsync(Guid customerId, Guid bookingId, UpdateBookingDto dto)
  {
    try
    {
      var parsedBookingDate = DateOnly.Parse(dto.BookingDate);
      var booking = await _unitOfWork.BookingRepository.GetAllQueryable()
          .Include(b => b.Slot)
          .ThenInclude(s => s.Bookinghuykts)
          .FirstOrDefaultAsync(b => b.Bookingid == bookingId && b.Customerid == customerId);

      if (booking is null)
        return ServiceResult.Failure("Booking is not found");

      // only allow edit if current status is PENDING
      if (!BookingStatusConstant.IsPending(booking.Status))
        return ServiceResult.Failure($"Cannot modify booking unless it is in #{BookingStatusConstant.Pending} state");

      if (dto.IsCancel)
      {
        booking.Status = BookingStatusConstant.Canceled;
        booking.Isactive = false;
        booking.Updatedat = DateTime.UtcNow;

        // optionally log status change elsewhere
        await _unitOfWork.SaveChangesAsync();
        await _bookingStatusLogService.AddLogAsync(bookingId);
        return ServiceResult.Success("Successfully");
      }

      // Update fields (SlotId change requires capacity check)
      if (dto.Slot != booking.Slot.Slot || parsedBookingDate != booking.BookingDate)
      {
        var newSlot = await _unitOfWork.BookingScheduleRepository.GetAllQueryable()
            .Include(s => s.Bookinghuykts)
            .FirstOrDefaultAsync(s => s.Centerid == dto.CenterId && dto.Slot == s.Slot);

        if (newSlot is null)
          return ServiceResult.Failure("New slot was not found");

        var currentCount = BookingScheduleService.NumberOfCurrentBookingRecord(newSlot, parsedBookingDate);

        if (newSlot.Capacity.HasValue && currentCount >= newSlot.Capacity.Value)
          return ServiceResult.Failure("The capacity for booking in this slot is full");

        booking.Slotid = newSlot.Slotid;
      }

      // vehicle exists and belong to user?
      var vehicleExists = await _unitOfWork.VehicleRepository.GetAllQueryable().AnyAsync(v => v.Vehicleid == dto.VehicleId && v.Customerid == customerId);
      if (!vehicleExists)
        return ServiceResult.Failure("Vehicle for this user was not found");

      booking.Vehicleid = dto.VehicleId;
      booking.Notes = dto.Notes ?? booking.Notes;
      booking.Updatedat = DateTime.UtcNow;

      await _unitOfWork.SaveChangesAsync();

      return ServiceResult.Success("Update successfully");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error in UpdateBookingAsync: {ex.Message}");
      return ServiceResult.Failure("Something was wrong");
    }
  }

  // Create booking information

  public async Task<IServiceResult<object>> CreateBookingAsync(Guid customerId, CreateBookingDto dto, CancellationToken ct = default)
  {
    try
    {
      // Load slot with booking records and center
      var slot = await _unitOfWork.BookingScheduleRepository.GetAllQueryable()
          .Include(s => s.Bookinghuykts)
          .Include(s => s.Center)
          .FirstOrDefaultAsync(s => s.Centerid == dto.CenterId && s.Slot == dto.Slot);

      if (slot is null)
        return ServiceResult<object>.Failure("Slot not found");

      // optional: ensure center matches payload if provided
      if (dto.CenterId.HasValue && dto.CenterId.Value != slot.Centerid)
        return ServiceResult<object>.Failure("Slot does not belong to provided center");

      // verify center exists (slot.Center already checked)
      if (slot.Center == null)
        return ServiceResult<object>.Failure("Center not found");

      // vehicle exists
      var vehicleExists = await _unitOfWork.VehicleRepository.GetAllQueryable()
          .AnyAsync(v => v.Vehicleid == dto.VehicleId && v.Customerid == customerId, ct);
      if (!vehicleExists)
        return ServiceResult<object>.Failure("Vehicle not found");

      // capacity check
      var currentCount = BookingScheduleService.NumberOfCurrentBookingRecord(slot, DateOnly.Parse(dto.BookingDate));
      if (slot.Capacity.HasValue && currentCount >= slot.Capacity.Value)
        return ServiceResult<object>.Failure("Slot is full");

      var newBooking = new Bookinghuykt
      {
        Bookingid = Guid.NewGuid(),
        Customerid = customerId,
        Vehicleid = dto.VehicleId,
        Slotid = slot.Slotid,
        Notes = dto.Notes,
        Status = BookingStatusConstant.Pending,
        Isactive = true,
        Createdat = DateTime.UtcNow,
        Updatedat = DateTime.UtcNow,
        BookingDate = DateOnly.Parse(dto.BookingDate)
      };

      _unitOfWork.BookingRepository.Add(newBooking);
      await _unitOfWork.SaveChangesAsync(ct);

      // Handle save log
      await _bookingStatusLogService.AddLogAsync(newBooking.Bookingid);

      return ServiceResult<object>.Success(new { Id = newBooking.Bookingid }, "Create booking successfully");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error in CreateBookingAsync: {ex.Message}");
      return ServiceResult<object>.Failure("Something was wrong");
    }
  }
}
