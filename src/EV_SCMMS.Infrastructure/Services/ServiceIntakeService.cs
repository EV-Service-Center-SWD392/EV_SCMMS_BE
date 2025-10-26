using System;
using System.Collections.Generic;
using System.Linq;
using EV_SCMMS.Core.Application.DTOs.ServiceIntake;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Service layer for Service Intake & EV checklist intake lifecycle
/// </summary>
public class ServiceIntakeService : IServiceIntakeService
{
    private static readonly HashSet<string> EditableStates = new(StringComparer.OrdinalIgnoreCase) { "CHECKED_IN", "INSPECTING" };
    private readonly IUnitOfWork _unitOfWork;

    public ServiceIntakeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<ServiceIntakeDto>> CreateAsync(CreateServiceIntakeDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto == null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Payload is required");
            }

            if (dto.CenterId == Guid.Empty || dto.VehicleId == Guid.Empty || dto.TechnicianId == Guid.Empty)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("CenterId, VehicleId and TechnicianId are required");
            }

            var (booking, assignmentResult, error) = await ResolveBookingAsync(dto, ct);
            if (error != null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure(error);
            }

            if (booking == null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Booking not found");
            }

            if (booking.Vehicleid != dto.VehicleId)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Vehicle does not match booking");
            }

            if (booking.Slot?.Centerid != null && booking.Slot.Centerid != dto.CenterId)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Center does not match booking slot");
            }

            var existingIntake = await _unitOfWork.ServiceIntakeRepository.GetAllQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(si => si.Bookingid == booking.Bookingid && si.Isactive != false, ct);
            if (existingIntake != null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("An active intake already exists for this booking");
            }

            if (assignmentResult != null && assignmentResult.Value != Guid.Empty)
            {
                var hasActive = await _unitOfWork.ServiceIntakeRepository.ExistsActiveByAssignmentAsync(assignmentResult.Value, ct);
                if (hasActive)
                {
                    return ServiceResult<ServiceIntakeDto>.Failure("Another active intake already uses this assignment");
                }
            }

            dto.BookingId = booking.Bookingid;

            var entity = dto.ToEntity();
            entity.Status = "CHECKED_IN";
            entity.Isactive = true;
            entity.Createdat = DateTime.UtcNow;
            entity.Updatedat = entity.Createdat;

            await _unitOfWork.ServiceIntakeRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            var reloaded = await _unitOfWork.ServiceIntakeRepository.GetByIdWithIncludesAsync(entity.Intakeid, ct) ?? entity;
            reloaded.Booking = reloaded.Booking ?? booking;

            var resultDto = reloaded.ToDto();
            return ServiceResult<ServiceIntakeDto>.Success(resultDto, "Service intake created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<ServiceIntakeDto>.Failure($"Error creating service intake: {ex.Message}");
        }
    }

    public async Task<IServiceResult<ServiceIntakeDto>> UpdateAsync(Guid id, UpdateServiceIntakeDto dto, CancellationToken ct = default)
    {
        try
        {
            var entity = await LoadTrackedIntakeAsync(id, ct);
            if (entity == null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Intake not found");
            }

            if (!EditableStates.Contains(entity.Status ?? string.Empty))
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Only CHECKED_IN or INSPECTING intakes can be updated");
            }

            entity.UpdateFromDto(dto);
            await _unitOfWork.SaveChangesAsync(ct);

            var refreshed = await _unitOfWork.ServiceIntakeRepository.GetByIdWithIncludesAsync(id, ct) ?? entity;
            var dtoOut = refreshed.ToDto();
            return ServiceResult<ServiceIntakeDto>.Success(dtoOut, "Intake updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<ServiceIntakeDto>.Failure($"Error updating intake: {ex.Message}");
        }
    }

    public async Task<IServiceResult<ServiceIntakeDto>> VerifyAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var entity = await LoadTrackedIntakeAsync(id, ct);
            if (entity == null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Intake not found");
            }

            var currentStatus = entity.Status ?? string.Empty;
            if (currentStatus.Equals("VERIFIED", StringComparison.OrdinalIgnoreCase) ||
                currentStatus.Equals("FINALIZED", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Intake already verified or finalized");
            }

            if (currentStatus.Equals("CHECKED_IN", StringComparison.OrdinalIgnoreCase))
            {
                var responses = await _unitOfWork.ChecklistRepository.GetResponsesAsync(id, ct);
                if (responses.Count > 0)
                {
                    return ServiceResult<ServiceIntakeDto>.Failure("Intake must be in INSPECTING status before verification");
                }
            }
            else if (!currentStatus.Equals("INSPECTING", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Only INSPECTING intakes can be verified");
            }

            entity.Status = "VERIFIED";
            entity.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(ct);

            var refreshed = await _unitOfWork.ServiceIntakeRepository.GetByIdWithIncludesAsync(id, ct) ?? entity;
            return ServiceResult<ServiceIntakeDto>.Success(refreshed.ToDto(), "Intake verified successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<ServiceIntakeDto>.Failure($"Error verifying intake: {ex.Message}");
        }
    }

    public async Task<IServiceResult<ServiceIntakeDto>> FinalizeAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var entity = await LoadTrackedIntakeAsync(id, ct);
            if (entity == null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Intake not found");
            }

            if (!string.Equals(entity.Status, "VERIFIED", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Only VERIFIED intakes can be finalized");
            }

            entity.Status = "FINALIZED";
            entity.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(ct);

            var refreshed = await _unitOfWork.ServiceIntakeRepository.GetByIdWithIncludesAsync(id, ct) ?? entity;
            return ServiceResult<ServiceIntakeDto>.Success(refreshed.ToDto(), "Intake finalized successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<ServiceIntakeDto>.Failure($"Error finalizing intake: {ex.Message}");
        }
    }

    public async Task<IServiceResult<ServiceIntakeDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _unitOfWork.ServiceIntakeRepository.GetByIdWithIncludesAsync(id, ct);
            if (entity == null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Intake not found");
            }

            return ServiceResult<ServiceIntakeDto>.Success(entity.ToDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<ServiceIntakeDto>.Failure($"Error retrieving intake: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<ServiceIntakeDto>>> GetRangeAsync(Guid? centerId, DateOnly? date, string? status, Guid? technicianId, CancellationToken ct = default)
    {
        try
        {
            var entities = await _unitOfWork.ServiceIntakeRepository.GetRangeAsync(centerId, date, status, technicianId, ct);
            var list = entities.ToDto();
            return ServiceResult<List<ServiceIntakeDto>>.Success(list);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<ServiceIntakeDto>>.Failure($"Error retrieving intakes: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> CancelAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var entity = await LoadTrackedIntakeAsync(id, ct);
            if (entity == null)
            {
                return ServiceResult<bool>.Failure("Intake not found");
            }

            if (!string.Equals(entity.Status, "CHECKED_IN", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<bool>.Failure("Only CHECKED_IN intakes can be cancelled");
            }

            entity.Status = "CANCELLED";
            entity.Isactive = false;
            entity.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(ct);
            return ServiceResult<bool>.Success(true, "Intake cancelled successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error cancelling intake: {ex.Message}");
        }
    }

    private async Task<(Bookinghuykt? Booking, Guid? AssignmentId, string? Error)> ResolveBookingAsync(CreateServiceIntakeDto dto, CancellationToken ct)
    {
        Guid? bookingId = dto.BookingId;
        Guid? assignmentId = dto.AssignmentId;

        if (!bookingId.HasValue || bookingId.Value == Guid.Empty)
        {
            if (!assignmentId.HasValue || assignmentId.Value == Guid.Empty)
            {
                return (null, null, "BookingId or AssignmentId is required");
            }

            var assignment = await _unitOfWork.AssignmentRepository.GetAllQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Assignmentid == assignmentId.Value, ct);
            if (assignment == null)
            {
                return (null, null, "Assignment not found");
            }

            if (assignment.Technicianid != dto.TechnicianId)
            {
                return (null, null, "Technician does not match assignment");
            }

            bookingId = assignment.Bookingid;
        }

        var booking = await _unitOfWork.BookingRepository.GetAllQueryable()
            .AsNoTracking()
            .Include(b => b.Slot)
            .Include(b => b.Vehicle)
            .Include(b => b.Assignmentthaontts)
            .FirstOrDefaultAsync(b => b.Bookingid == bookingId.Value, ct);

        if (booking == null)
        {
            return (null, assignmentId, "Booking not found");
        }

        return (booking, assignmentId, null);
    }

    private async Task<Serviceintakethaontt?> LoadTrackedIntakeAsync(Guid id, CancellationToken ct)
    {
        return await _unitOfWork.ServiceIntakeRepository.GetAllQueryable()
            .Include(si => si.Booking)
                .ThenInclude(b => b.Slot)
            .Include(si => si.Booking)
                .ThenInclude(b => b.Vehicle)
            .Include(si => si.Booking)
                .ThenInclude(b => b.Assignmentthaontts)
            .FirstOrDefaultAsync(si => si.Intakeid == id, ct);
    }
}
