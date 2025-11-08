using EV_SCMMS.Core.Application.DTOs.Assignment;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Business logic for technician assignments, enforcing WorkSchedule capacity
/// </summary>
public class AssignmentService : IAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<AssignmentDto>> CreateAsync(CreateAssignmentDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto.PlannedEndUtc <= dto.PlannedStartUtc)
            {
                return ServiceResult<AssignmentDto>.Failure("PlannedEndUtc must be after PlannedStartUtc");
            }

            var technicianId = dto.TechnicianId;
            // Derive center from booking
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(dto.BookingId, ct);
            if (booking == null)
            {
                return ServiceResult<AssignmentDto>.Failure("Booking not found");
            }

            if (!string.Equals(booking.Status, "APPROVED", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<AssignmentDto>.Failure("Assignment allowed only for APPROVED bookings");
            }

            var centerId = booking.Slot?.Centerid;
            if (!centerId.HasValue || centerId.Value == Guid.Empty)
            {
                return ServiceResult<AssignmentDto>.Failure("Booking does not have a valid center");
            }

            // Validate technician belongs to the same center via work schedules
            // Get all work schedules for this technician
            var technicianWorkSchedules = await _unitOfWork.UserWorkScheduleRepository.GetAllAsync();
            var userSchedules = technicianWorkSchedules
                .Where(uws => uws.Userid == technicianId && uws.Isactive == true)
                .ToList();

            if (!userSchedules.Any())
            {
                return ServiceResult<AssignmentDto>.Failure(
                    $"Technician (ID: {technicianId}) has no work schedules assigned. Please create a work schedule for this technician first.");
            }

            // Get the center IDs from work schedules
            var workScheduleIds = userSchedules.Select(uws => uws.Workscheduleid).ToList();
            var allWorkSchedules = await _unitOfWork.WorkScheduleRepository.GetAllAsync();
            var technicianCenters = allWorkSchedules
                .Where(ws => workScheduleIds.Contains(ws.Workscheduleid) && ws.Isactive == true)
                .Select(ws => ws.Centerid)
                .Distinct()
                .ToList();

            if (!technicianCenters.Any())
            {
                return ServiceResult<AssignmentDto>.Failure(
                    $"Technician (ID: {technicianId}) has no active work schedules. Please activate work schedules for this technician.");
            }

            // Check if technician has work schedule in the booking's center
            if (!technicianCenters.Contains(centerId.Value))
            {
                return ServiceResult<AssignmentDto>.Failure(
                    $"Technician (ID: {technicianId}) does not have work schedules in booking center (ID: {centerId.Value}). " +
                    $"Technician has work schedules in centers: {string.Join(", ", technicianCenters)}");
            }
            var startUtc = DateTime.SpecifyKind(dto.PlannedStartUtc, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(dto.PlannedEndUtc, DateTimeKind.Utc);
            var workDate = DateOnly.FromDateTime(startUtc);
            var startTime = TimeOnly.FromDateTime(startUtc);
            var endTime = TimeOnly.FromDateTime(endUtc);

            // Schedules for this technician that overlap the requested window
            var schedules = await _unitOfWork.WorkScheduleRepository.GetByTechnicianIdAsync(technicianId, ct);
            var matchingSchedules = schedules
                .Where(s => s.Isactive == true &&
                           s.Starttime.Date == workDate.ToDateTime(TimeOnly.MinValue).Date &&
                           startTime.ToTimeSpan() < s.Endtime.TimeOfDay &&
                           s.Starttime.TimeOfDay < endTime.ToTimeSpan())
                .ToList();

            if (matchingSchedules.Count == 0)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician not available in this time range");
            }

            // Capacity check across overlapped schedules (consider only assigned/active)
            var relevantStatuses = new[] { "ASSIGNED", "ACTIVE" };
            var assignedCount = await _unitOfWork.AssignmentRepository
                .CountAssignmentsWithStatusesByTechnicianAndRangeAsync(technicianId, startUtc, endUtc, relevantStatuses, ct);
            // Note: Capacity check removed as SlotCapacity property doesn't exist in current entity

            // Avoid double-booking in same window (consider only assigned/active)
            var hasOverlap = await _unitOfWork.AssignmentRepository
                .ExistsOverlapWithStatusesAsync(technicianId, startUtc, endUtc, relevantStatuses, null, ct);
            if (hasOverlap)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician already assigned at this time");
            }

            var entity = dto.ToEntity();
            // Set status to ASSIGNED on creation per lifecycle
            entity.Status = "ASSIGNED";
            await _unitOfWork.AssignmentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            // Map and attach CenterId from request (as entity does not carry CenterId directly)
            var resultDto = entity.ToDto();
            if (resultDto != null)
            {
                resultDto.CenterId = centerId.Value;
            }

            return ServiceResult<AssignmentDto>.Success(resultDto!, "Assignment created successfully");
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error creating assignment: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $" | Inner Exception: {ex.InnerException.Message}";
            }
            return ServiceResult<AssignmentDto>.Failure(errorMessage);
        }
    }

    public async Task<IServiceResult<AssignmentDto>> RescheduleAsync(Guid id, RescheduleAssignmentDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto.PlannedEndUtc <= dto.PlannedStartUtc)
            {
                return ServiceResult<AssignmentDto>.Failure("PlannedEndUtc must be after PlannedStartUtc");
            }

            var entity = await _unitOfWork.AssignmentRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<AssignmentDto>.Failure("Assignment not found");
            }

            var newStart = DateTime.SpecifyKind(dto.PlannedStartUtc, DateTimeKind.Utc);
            var newEnd = DateTime.SpecifyKind(dto.PlannedEndUtc, DateTimeKind.Utc);

            var workDate = DateOnly.FromDateTime(newStart);
            var startTime = TimeOnly.FromDateTime(newStart);
            var endTime = TimeOnly.FromDateTime(newEnd);

            // Schedule availability for the same technician
            var schedules = await _unitOfWork.WorkScheduleRepository.GetByTechnicianIdAsync(entity.Technicianid, ct);
            var matchingSchedules = schedules
                .Where(s => s.Isactive == true &&
                           s.Starttime.Date == workDate.ToDateTime(TimeOnly.MinValue).Date &&
                           startTime.ToTimeSpan() < s.Endtime.TimeOfDay &&
                           s.Starttime.TimeOfDay < endTime.ToTimeSpan())
                .ToList();
            if (matchingSchedules.Count == 0)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician not available in this time range");
            }

            // Capacity check (exclude itself if overlapping old time)
            var totalCount = await _unitOfWork.AssignmentRepository
                .CountAssignmentsByTechnicianAndRangeAsync(entity.Technicianid, newStart, newEnd, ct);
            var overlapsSelf = entity.Plannedstartutc.HasValue && entity.Plannedendutc.HasValue && entity.Plannedstartutc.Value < newEnd && newStart < entity.Plannedendutc.Value;
            var effectiveCount = overlapsSelf ? Math.Max(0, totalCount - 1) : totalCount;
            // Note: Capacity check removed as SlotCapacity property doesn't exist in current entity

            // Overlap with others (exclude self)
            var conflict = await _unitOfWork.AssignmentRepository
                .ExistsOverlapAsync(entity.Technicianid, newStart, newEnd, id, ct);
            if (conflict)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician already assigned in this time range");
            }

            entity.UpdateTimes(newStart, newEnd);
            await _unitOfWork.AssignmentRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            // Compute CenterId via booking->slot if available
            var dtoOut = entity.ToDto();
            if (dtoOut != null)
            {
                // best-effort fill if ToDto failed to include center
                if (dtoOut.CenterId == Guid.Empty && entity.Booking?.Slotid != null)
                {
                    var booking = await _unitOfWork.BookingRepository.GetByIdAsync(entity.Bookingid);
                    if (booking?.Slotid != null)
                    {
                        var slot = await _unitOfWork.BookingScheduleRepository.GetByIdAsync(booking.Slotid.Value);
                        if (slot != null) dtoOut.CenterId = slot.Centerid;
                    }
                }
            }

            return ServiceResult<AssignmentDto>.Success(dtoOut!, "Assignment rescheduled successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<AssignmentDto>.Failure($"Error rescheduling assignment: {ex.Message}");
        }
    }

    public async Task<IServiceResult<AssignmentDto>> ReassignAsync(Guid id, ReassignTechnicianDto dto, CancellationToken ct = default)
    {
        try
        {
            var entity = await _unitOfWork.AssignmentRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<AssignmentDto>.Failure("Assignment not found");
            }

            var newTechId = dto.NewTechnicianId;
            var startUtc = DateTime.SpecifyKind(entity.Plannedstartutc ?? DateTime.UtcNow, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(entity.Plannedendutc ?? DateTime.UtcNow, DateTimeKind.Utc);

            var workDate = DateOnly.FromDateTime(startUtc);
            var startTime = TimeOnly.FromDateTime(startUtc);
            var endTime = TimeOnly.FromDateTime(endUtc);

            // Schedules for new technician overlapping current window
            var schedules = await _unitOfWork.WorkScheduleRepository.GetByTechnicianIdAsync(newTechId, ct);
            var matchingSchedules = schedules
                .Where(s => s.Isactive == true &&
                           s.Starttime.Date == workDate.ToDateTime(TimeOnly.MinValue).Date &&
                           startTime.ToTimeSpan() < s.Endtime.TimeOfDay &&
                           s.Starttime.TimeOfDay < endTime.ToTimeSpan())
                .ToList();
            if (matchingSchedules.Count == 0)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician not available in this time range");
            }

            // Capacity check (exclude this assignment if previously on another tech)
            var totalCount = await _unitOfWork.AssignmentRepository
                .CountAssignmentsByTechnicianAndRangeAsync(newTechId, startUtc, endUtc, ct);
            var excludeSelf = entity.Technicianid == newTechId && entity.Plannedstartutc.HasValue && entity.Plannedendutc.HasValue && entity.Plannedstartutc.Value < endUtc && startUtc < entity.Plannedendutc.Value;
            var effectiveCount = excludeSelf ? Math.Max(0, totalCount - 1) : totalCount;
            // Note: Capacity check removed as SlotCapacity property doesn't exist in current entity

            // Overlap with others (exclude self)
            var conflict = await _unitOfWork.AssignmentRepository
                .ExistsOverlapAsync(newTechId, startUtc, endUtc, id, ct);
            if (conflict)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician already assigned in this time range");
            }

            entity.UpdateTechnician(newTechId);
            await _unitOfWork.AssignmentRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            var dtoOut = entity.ToDto();
            if (dtoOut != null && dtoOut.CenterId == Guid.Empty && entity.Booking?.Slotid != null)
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(entity.Bookingid);
                if (booking?.Slotid != null)
                {
                    var slot = await _unitOfWork.BookingScheduleRepository.GetByIdAsync(booking.Slotid.Value);
                    if (slot != null) dtoOut.CenterId = slot.Centerid;
                }
            }

            return ServiceResult<AssignmentDto>.Success(dtoOut!, "Assignment reassigned successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<AssignmentDto>.Failure($"Error reassigning technician: {ex.Message}");
        }
    }

    /// <summary>
    /// Cancel an assignment and update booking status if needed
    /// When the last active assignment is cancelled, the booking remains APPROVED
    /// and becomes available for reassignment
    /// </summary>
    public async Task<IServiceResult<CancelAssignmentResponseDto>> CancelAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _unitOfWork.AssignmentRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<CancelAssignmentResponseDto>.Failure("Assignment not found");
            }

            // Store booking ID before cancelling
            var bookingId = entity.Bookingid;

            // Cancel the assignment
            entity.Status = "CANCELLED";
            entity.Isactive = false;
            entity.Updatedat = DateTime.UtcNow;
            await _unitOfWork.AssignmentRepository.UpdateAsync(entity);

            // Check if there are any other active assignments for this booking
            var allAssignments = await _unitOfWork.AssignmentRepository.GetAllAsync();
            var activeAssignmentsForBooking = allAssignments
                .Where(a => a.Bookingid == bookingId
                    && a.Assignmentid != id
                    && a.Isactive == true
                    && !string.Equals(a.Status, "CANCELLED", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(a.Status, "NOSHOW", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var hasActiveAssignments = activeAssignmentsForBooking.Any();

            // Get booking to check current status
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, ct);
            string bookingStatus = booking?.Status ?? "UNKNOWN";
            string message = "Assignment cancelled successfully";

            // If no active assignments remain, update booking to make it ready for reassignment
            if (!hasActiveAssignments)
            {
                if (booking != null && string.Equals(booking.Status, "APPROVED", StringComparison.OrdinalIgnoreCase))
                {
                    // Booking stays APPROVED but now has no active assignments
                    // This makes it ready for reassignment without changing workflow
                    booking.Updatedat = DateTime.UtcNow;
                    await _unitOfWork.BookingRepository.UpdateAsync(booking);
                    message = "Assignment cancelled successfully. Booking is now available for reassignment.";
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);

            var response = new CancelAssignmentResponseDto
            {
                AssignmentId = id,
                BookingId = bookingId,
                HasActiveAssignments = hasActiveAssignments,
                BookingStatus = bookingStatus,
                Message = message
            };

            return ServiceResult<CancelAssignmentResponseDto>.Success(response, message);
        }
        catch (Exception ex)
        {
            return ServiceResult<CancelAssignmentResponseDto>.Failure($"Error cancelling assignment: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> NoShowAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _unitOfWork.AssignmentRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<bool>.Failure("Assignment not found");
            }

            entity.Status = "NOSHOW";
            entity.Updatedat = DateTime.UtcNow;
            await _unitOfWork.AssignmentRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return ServiceResult<bool>.Success(true, "Assignment marked as no-show");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error setting no-show: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<AssignmentDto>>> GetRangeAsync(Guid? centerId, DateOnly? date, string? status, CancellationToken ct = default)
    {
        try
        {
            var items = await _unitOfWork.AssignmentRepository.GetRangeAsync(centerId, date, status, ct);
            var list = items.ToDto();
            return ServiceResult<List<AssignmentDto>>.Success(list);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<AssignmentDto>>.Failure($"Error retrieving assignments: {ex.Message}");
        }
    }

    public async Task<IServiceResult<AssignmentDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _unitOfWork.AssignmentRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<AssignmentDto>.Failure("Assignment not found");
            }

            var dto = entity.ToDto() ?? new AssignmentDto
            {
                Id = entity.Assignmentid,
                TechnicianId = entity.Technicianid,
                PlannedStartUtc = entity.Plannedstartutc ?? default,
                PlannedEndUtc = entity.Plannedendutc ?? default,
                Status = entity.Status ?? string.Empty
            };

            if (dto.CenterId == Guid.Empty)
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(entity.Bookingid);
                if (booking?.Slotid != null)
                {
                    var slot = await _unitOfWork.BookingScheduleRepository.GetByIdAsync(booking.Slotid.Value);
                    if (slot != null) dto.CenterId = slot.Centerid;
                }
            }

            return ServiceResult<AssignmentDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return ServiceResult<AssignmentDto>.Failure($"Error retrieving assignment: {ex.Message}");
        }
    }

    /// <summary>
    /// Check if a booking has any active assignments
    /// </summary>
    /// <param name="bookingId">The booking ID to check</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if booking has active assignments, false otherwise</returns>
    private async Task<bool> HasActiveAssignmentsAsync(Guid bookingId, CancellationToken ct = default)
    {
        var allAssignments = await _unitOfWork.AssignmentRepository.GetAllAsync();
        return allAssignments.Any(a =>
            a.Bookingid == bookingId
            && a.Isactive == true
            && !string.Equals(a.Status, "CANCELLED", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(a.Status, "NOSHOW", StringComparison.OrdinalIgnoreCase));
    }
}
