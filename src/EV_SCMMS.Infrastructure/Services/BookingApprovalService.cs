using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    private static readonly string[] PendingStatuses = { "PENDING", "REQUESTED" };
    private readonly IUnitOfWork _unitOfWork;

    public BookingApprovalService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

    public async Task<IServiceResult<List<BookingApprovalDto>>> GetPendingAsync(Guid? centerId, DateOnly? date, CancellationToken ct = default)
    {
        try
        {
            var bookings = await _unitOfWork.BookingRepository.GetPendingAsync(centerId, date, ct);
            var dtos = bookings.ToBookingApprovalDto().ToList();
            return ServiceResult<List<BookingApprovalDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<BookingApprovalDto>>.Failure($"Error retrieving pending bookings: {ex.Message}");
        }
    }

    public async Task<IServiceResult<BookingApprovalDto>> ApproveAsync(ApproveBookingDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto.BookingId == Guid.Empty)
            {
                return ServiceResult<BookingApprovalDto>.Failure("BookingId is required");
            }

            if (dto.StaffId == Guid.Empty)
            {
                return ServiceResult<BookingApprovalDto>.Failure("StaffId is required");
            }

            var booking = await LoadBookingForUpdateAsync(dto.BookingId, ct);
            if (booking == null)
            {
                return ServiceResult<BookingApprovalDto>.Failure("Booking not found");
            }

            if (!IsPending(booking.Status))
            {
                return ServiceResult<BookingApprovalDto>.Failure("Only pending bookings can be approved");
            }

            var (startUtc, endUtc) = GetTimeWindow(booking);
            if (!startUtc.HasValue || !endUtc.HasValue)
            {
                return ServiceResult<BookingApprovalDto>.Failure("Booking must include a preferred time window");
            }

            if (startUtc.Value >= endUtc.Value)
            {
                return ServiceResult<BookingApprovalDto>.Failure("PreferredStartUtc must be earlier than PreferredEndUtc");
            }

            var centerId = booking.Slot?.Centerid;
            if (!centerId.HasValue)
            {
                return ServiceResult<BookingApprovalDto>.Failure("Booking must specify a service center");
            }

            var normalizedStart = EnsureUtc(startUtc.Value);
            var normalizedEnd = EnsureUtc(endUtc.Value);

            var hasConflict = await _unitOfWork.BookingRepository
                .ExistsApprovedOverlapAsync(centerId.Value, normalizedStart, normalizedEnd, ct);
            if (hasConflict)
            {
                return ServiceResult<BookingApprovalDto>.Failure("Time slot conflict at center");
            }

            var now = DateTime.UtcNow;
            booking.Status = "APPROVED";
            booking.Updatedat = now;

            if (!string.IsNullOrWhiteSpace(dto.Note))
            {
                booking.Notes = dto.Note;
            }

            await _unitOfWork.SaveChangesAsync(ct);

            // Optional queue integration could be placed here when available

            return ServiceResult<BookingApprovalDto>.Success(booking.ToBookingApprovalDto(), "Booking approved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<BookingApprovalDto>.Failure($"Error approving booking: {ex.Message}");
        }
    }

    public async Task<IServiceResult<BookingApprovalDto>> RejectAsync(RejectBookingDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto.BookingId == Guid.Empty)
            {
                return ServiceResult<BookingApprovalDto>.Failure("BookingId is required");
            }

            if (dto.StaffId == Guid.Empty)
            {
                return ServiceResult<BookingApprovalDto>.Failure("StaffId is required");
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

            if (!IsPending(booking.Status))
            {
                return ServiceResult<BookingApprovalDto>.Failure("Only pending bookings can be rejected");
            }

            var now = DateTime.UtcNow;
            booking.Status = "REJECTED";
            booking.Updatedat = now;
            booking.Notes = dto.Reason;

            await _unitOfWork.SaveChangesAsync(ct);

            return ServiceResult<BookingApprovalDto>.Success(booking.ToBookingApprovalDto(), "Booking rejected successfully");
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

    private static bool IsPending(string? status)
    {
        if (string.IsNullOrWhiteSpace(status)) return false;
        return PendingStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }

    private static (DateTime?, DateTime?) GetTimeWindow(Bookinghuykt booking)
    {
        if (booking.Slot == null) return (null, null);
        return (booking.Slot.Startutc, booking.Slot.Endutc);
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}
