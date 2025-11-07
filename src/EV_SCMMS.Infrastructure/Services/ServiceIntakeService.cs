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

    public async Task<IServiceResult<ServiceIntakeDto>> CreateAsync(CreateServiceIntakeDto dto, Guid checkedInBy, CancellationToken ct = default)
    {
        try
        {
            if (dto == null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Payload is required");
            }

            if (dto.BookingId == Guid.Empty)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("BookingId is required");
            }

            var booking = await _unitOfWork.BookingRepository.GetAllQueryable()
                .AsNoTracking()
                .Include(b => b.Slot)
                .Include(b => b.Vehicle)
                .FirstOrDefaultAsync(b => b.Bookingid == dto.BookingId, ct);

            if (booking == null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Booking not found");
            }

            // Load assignments separately to avoid querying non-existent note column
            var assignments = await _unitOfWork.AssignmentRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(a => a.Bookingid == booking.Bookingid)
                .Select(a => new Assignmentthaontt
                {
                    Assignmentid = a.Assignmentid,
                    Bookingid = a.Bookingid,
                    Technicianid = a.Technicianid,
                    Queueno = a.Queueno,
                    Plannedstartutc = a.Plannedstartutc,
                    Plannedendutc = a.Plannedendutc,
                    Status = a.Status,
                    Isactive = a.Isactive,
                    Createdat = a.Createdat,
                    Updatedat = a.Updatedat
                })
                .ToListAsync(ct);
            booking.Assignmentthaontts = assignments;

            // Optional business rule: allow intake creation only for approved bookings
            if (!string.Equals(booking.Status, "APPROVED", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Service intake allowed only for APPROVED bookings");
            }

            var existingIntake = await _unitOfWork.ServiceIntakeRepository.GetAllQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(si => si.Bookingid == booking.Bookingid && si.Isactive != false, ct);
            if (existingIntake != null)
            {
                return ServiceResult<ServiceIntakeDto>.Failure("An active intake already exists for this booking");
            }

            var entity = dto.ToEntity();
            entity.Bookingid = booking.Bookingid;
            entity.CheckedInBy = checkedInBy; // store check-in actor
            entity.Status = "CHECKED_IN";
            entity.Isactive = true;
            entity.Createdat = DateTime.UtcNow;
            entity.Updatedat = entity.Createdat;

            await _unitOfWork.ServiceIntakeRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            // Transition related assignments to ACTIVE when customer checks in
            try
            {
                var now = DateTime.UtcNow;
                var activeAssignments = await _unitOfWork.AssignmentRepository
                    .GetAllQueryable()
                    .Where(a => a.Bookingid == booking.Bookingid && a.Isactive == true)
                    .ToListAsync(ct);
                foreach (var a in activeAssignments)
                {
                    a.Status = "ACTIVE";
                    a.Updatedat = now;
                }
                if (activeAssignments.Count > 0)
                {
                    await _unitOfWork.SaveChangesAsync(ct);
                }
            }
            catch { /* best-effort; do not fail intake creation */ }

            // Map to DTO without reloading to avoid querying assignment.note column
            entity.Booking = booking;
            var resultDto = entity.ToDto();
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

            if (!currentStatus.Equals("INSPECTING", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<ServiceIntakeDto>.Failure("Intake must be in INSPECTING status before verification");
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

    public async Task<IServiceResult<List<ServiceIntakeDto>>> GetRangeAsync(Guid? centerId, DateOnly? date, Guid? technicianId, CancellationToken ct = default)
    {
        try
        {
            var entities = await _unitOfWork.ServiceIntakeRepository.GetRangeAsync(centerId, date, technicianId, ct);
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

    // Booking resolution via Assignment is no longer needed as Create requires BookingId only.

    private async Task<Serviceintakethaontt?> LoadTrackedIntakeAsync(Guid id, CancellationToken ct)
    {
        var intake = await _unitOfWork.ServiceIntakeRepository.GetAllQueryable()
            .Include(si => si.Booking)
                .ThenInclude(b => b.Slot)
            .Include(si => si.Booking)
                .ThenInclude(b => b.Vehicle)
            .FirstOrDefaultAsync(si => si.Intakeid == id, ct);

        if (intake?.Booking != null)
        {
            // Load assignments separately to avoid querying non-existent note column
            var assignments = await _unitOfWork.AssignmentRepository.GetAllQueryable()
                .Where(a => a.Bookingid == intake.Booking.Bookingid)
                .Select(a => new Assignmentthaontt
                {
                    Assignmentid = a.Assignmentid,
                    Bookingid = a.Bookingid,
                    Technicianid = a.Technicianid,
                    Queueno = a.Queueno,
                    Plannedstartutc = a.Plannedstartutc,
                    Plannedendutc = a.Plannedendutc,
                    Status = a.Status,
                    Isactive = a.Isactive,
                    Createdat = a.Createdat,
                    Updatedat = a.Updatedat
                })
                .ToListAsync(ct);
            intake.Booking.Assignmentthaontts = assignments;
        }

        return intake;
    }
}
