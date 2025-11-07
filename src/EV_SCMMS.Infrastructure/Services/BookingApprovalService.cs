using System.Globalization;
using EV_SCMMS.Core.Application.DTOs.BookingApproval;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Handles staff-side booking approval and rejection workflow.
/// </summary>
public class BookingApprovalService : IBookingApprovalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BookingStatusLogService _bookingStatusLogService;

    public BookingApprovalService(IUnitOfWork unitOfWork, BookingStatusLogService bookingStatusLogService)
    {
        _unitOfWork = unitOfWork;
        _bookingStatusLogService = bookingStatusLogService;
    }

    public async Task<IServiceResult<BookingApprovalDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id, ct);
            if (booking == null)
            {
                return ServiceResult<BookingApprovalDto>.Failure("Booking not found");
            }

            return ServiceResult<BookingApprovalDto>.Success(booking.ToBookingApprovalDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<BookingApprovalDto>.Failure($"Error retrieving booking: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<BookingApprovalDto>>> GetPendingAsync(Guid? centerId, CenterSchedulesQueryDto? dto, CancellationToken ct = default)
    {
        DateOnly? startDate = null;
        DateOnly? endDate = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(dto.StartDate))
                startDate = DateOnly.Parse(dto.StartDate);

            if (!string.IsNullOrWhiteSpace(dto.EndDate))
                endDate = DateOnly.Parse(dto.EndDate);

            var bookings = await _unitOfWork.BookingRepository.GetPendingAsync(centerId, startDate, endDate, ct);
            var dtos = bookings.ToBookingApprovalDto().ToList();
            return ServiceResult<List<BookingApprovalDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<BookingApprovalDto>>.Failure($"Error retrieving pending bookings: {ex.Message}");
        }
    }

    private IServiceResult<List<BookingApprovalDto>> BadRequest(string v)
    {
        throw new NotImplementedException();
    }

    public async Task<IServiceResult<BookingApprovalDto>> ApproveAsync(ApproveBookingDto dto, Guid staffId, CancellationToken ct = default)
    {
        try
        {
            if (dto.BookingId == Guid.Empty)
            {
                return ServiceResult<BookingApprovalDto>.Failure("BookingId is required");
            }

            var booking = await LoadBookingForUpdateAsync(dto.BookingId, ct);
            if (booking == null)
            {
                return ServiceResult<BookingApprovalDto>.Failure("Booking not found");
            }

            if (!BookingStatusConstant.IsPending(booking.Status))
            {
                return ServiceResult<BookingApprovalDto>.Failure(
                    $"Only pending bookings can be approved. Current status: '{booking.Status}' (Length: {booking.Status?.Length ?? 0})");
            }

            var now = DateTime.UtcNow;
            booking.Status = BookingStatusConstant.Approved;
            booking.Updatedat = now;

            if (!string.IsNullOrWhiteSpace(dto.Note))
            {
                booking.Notes = dto.Note;
            }

            await _unitOfWork.SaveChangesAsync(ct);

            await _bookingStatusLogService.AddLogAsync(dto.BookingId);


            // Build response and include actor if applicable
            var response = booking.ToBookingApprovalDto();
            response.ApprovedBy = staffId;
            response.ApprovedAt = now;

            return ServiceResult<BookingApprovalDto>.Success(response, "Booking approved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<BookingApprovalDto>.Failure($"Error approving booking: {ex.Message}");
        }
    }

    public async Task<IServiceResult<BookingApprovalDto>> RejectAsync(RejectBookingDto dto, Guid staffId, CancellationToken ct = default)
    {
        try
        {
            if (dto.BookingId == Guid.Empty)
            {
                return ServiceResult<BookingApprovalDto>.Failure("BookingId is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                return ServiceResult<BookingApprovalDto>.Failure("Reject reason is required");
            }

            var booking = await LoadBookingForUpdateAsync(dto.BookingId, ct);

            if (booking == null)
            {
                return ServiceResult<BookingApprovalDto>.Failure("Booking not found");
            }

            if (!BookingStatusConstant.IsPending(booking.Status))
            {
                return ServiceResult<BookingApprovalDto>.Failure(
                    $"Only pending bookings can be rejected. Current status: '{booking.Status}' (Length: {booking.Status?.Length ?? 0})");
            }

            var now = DateTime.UtcNow;
            booking.Status = BookingStatusConstant.Rejected;
            booking.Updatedat = now;
            booking.Notes = dto.Reason;

            await _unitOfWork.SaveChangesAsync(ct);
            await _bookingStatusLogService.AddLogAsync(dto.BookingId);


            var response = booking.ToBookingApprovalDto();
            response.RejectedBy = staffId;
            response.RejectedAt = now;

            return ServiceResult<BookingApprovalDto>.Success(response, "Booking rejected successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<BookingApprovalDto>.Failure($"Error rejecting booking: {ex.Message}");
        }
    }



    private async Task<Bookinghuykt?> LoadBookingForUpdateAsync(Guid bookingId, CancellationToken ct)
    {
        return await _unitOfWork.BookingRepository
            .GetAllQueryable()
            .Include(b => b.Slot)
            .FirstOrDefaultAsync(b => b.Bookingid == bookingId, ct);
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}
