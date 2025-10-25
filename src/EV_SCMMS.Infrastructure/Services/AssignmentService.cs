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
            var startUtc = DateTime.SpecifyKind(dto.PlannedStartUtc, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(dto.PlannedEndUtc, DateTimeKind.Utc);
            var workDate = DateOnly.FromDateTime(startUtc);
            var startTime = TimeOnly.FromDateTime(startUtc);
            var endTime = TimeOnly.FromDateTime(endUtc);

            // Schedules for this technician that overlap the requested window
            var schedules = await _unitOfWork.WorkScheduleRepository.GetByTechnicianIdAsync(technicianId, ct);
            var matchingSchedules = schedules
                .Where(s => s.IsActive && s.WorkDate == workDate && startTime < s.EndTime && s.StartTime < endTime)
                .ToList();

            if (matchingSchedules.Count == 0)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician not available in this time range");
            }

            // Capacity check across overlapped schedules
            var assignedCount = await _unitOfWork.AssignmentRepository
                .CountAssignmentsByTechnicianAndRangeAsync(technicianId, startUtc, endUtc, ct);
            var hasCapacity = matchingSchedules.Any(s => s.SlotCapacity - assignedCount > 0);
            if (!hasCapacity)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician slot full");
            }

            // Avoid double-booking in same window
            var hasOverlap = await _unitOfWork.AssignmentRepository
                .ExistsOverlapAsync(technicianId, startUtc, endUtc, null, ct);
            if (hasOverlap)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician already assigned in this time range");
            }

            var entity = dto.ToEntity();
            await _unitOfWork.AssignmentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            // Map and attach CenterId from request (as entity does not carry CenterId directly)
            var resultDto = entity.ToDto();
            if (resultDto != null)
            {
                resultDto.CenterId = dto.CenterId;
            }

            return ServiceResult<AssignmentDto>.Success(resultDto!, "Assignment created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<AssignmentDto>.Failure($"Error creating assignment: {ex.Message}");
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
                .Where(s => s.IsActive && s.WorkDate == workDate && startTime < s.EndTime && s.StartTime < endTime)
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
            var hasCapacity = matchingSchedules.Any(s => s.SlotCapacity - effectiveCount > 0);
            if (!hasCapacity)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician slot full");
            }

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
                .Where(s => s.IsActive && s.WorkDate == workDate && startTime < s.EndTime && s.StartTime < endTime)
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
            var hasCapacity = matchingSchedules.Any(s => s.SlotCapacity - effectiveCount > 0);
            if (!hasCapacity)
            {
                return ServiceResult<AssignmentDto>.Failure("Technician slot full");
            }

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

    public async Task<IServiceResult<bool>> CancelAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _unitOfWork.AssignmentRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<bool>.Failure("Assignment not found");
            }

            entity.Status = "CANCELLED";
            entity.Isactive = false;
            entity.Updatedat = DateTime.UtcNow;
            await _unitOfWork.AssignmentRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return ServiceResult<bool>.Success(true, "Assignment cancelled successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error cancelling assignment: {ex.Message}");
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
}
