using EV_SCMMS.Core.Application.DTOs.WorkOrder;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Technician-driven Work Order lifecycle service (draft -> approval -> execution).
/// Entity model is not modified; state enforcement occurs at service layer.
/// </summary>
public class WorkOrderService : IWorkOrderService
{
    private static readonly HashSet<string> EditableStates = new(StringComparer.OrdinalIgnoreCase) { "DRAFT", "REVISED" };
    private readonly IUnitOfWork _unitOfWork;

    public WorkOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<WorkOrderDto>> CreateAsync(CreateWorkOrderDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto == null || dto.IntakeId == Guid.Empty)
            {
                return ServiceResult<WorkOrderDto>.Failure("IntakeId is required");
            }

            var intake = await _unitOfWork.ServiceIntakeRepository.GetAllQueryable()
                .Include(si => si.Booking)
                .FirstOrDefaultAsync(si => si.Intakeid == dto.IntakeId, ct);

            if (intake == null)
            {
                return ServiceResult<WorkOrderDto>.Failure("Service intake not found");
            }

            var status = intake.Status ?? string.Empty;
            if (!status.Equals("VERIFIED", StringComparison.OrdinalIgnoreCase) && !status.Equals("FINALIZED", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<WorkOrderDto>.Failure("Work Order can be created only when intake is VERIFIED or FINALIZED");
            }

            var hasActive = await _unitOfWork.WorkOrderRepository.ExistsActiveByIntakeAsync(dto.IntakeId, ct);
            if (hasActive)
            {
                return ServiceResult<WorkOrderDto>.Failure("An active Work Order already exists for this intake");
            }

            var entity = dto.ToEntity();
            entity.Status = "DRAFT";
            entity.Createdat = DateTime.UtcNow;
            entity.Updatedat = entity.Createdat;
            entity.Isactive = true;

            await _unitOfWork.WorkOrderRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            // Reload with includes for mapping
            var reloaded = await _unitOfWork.WorkOrderRepository.GetByIdAsync(entity.WoaId, ct) ?? entity;
            var dtoOut = reloaded.ToDto();
            // The following fields are not persisted; echo back from request if provided
            dtoOut.Title = dto.Title;
            dtoOut.Description = dto.Description;
            dtoOut.EstimatedAmount = dto.EstimatedAmount;
            dtoOut.Lines = dto.Lines;

            return ServiceResult<WorkOrderDto>.Success(dtoOut, "Work Order created in DRAFT");
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkOrderDto>.Failure($"Error creating Work Order: {ex.Message}");
        }
    }

    public async Task<IServiceResult<WorkOrderDto>> UpdateAsync(Guid id, UpdateWorkOrderDto dto, CancellationToken ct = default)
    {
        try
        {
            var entity = await LoadTrackedAsync(id, ct);
            if (entity == null)
            {
                return ServiceResult<WorkOrderDto>.Failure("Work Order not found");
            }

            if (!EditableStates.Contains(entity.Status ?? string.Empty))
            {
                return ServiceResult<WorkOrderDto>.Failure("Only DRAFT or REVISED Work Orders can be updated");
            }

            entity.UpdateFromDto(dto);
            await _unitOfWork.SaveChangesAsync(ct);

            var refreshed = await _unitOfWork.WorkOrderRepository.GetByIdAsync(id, ct) ?? entity;
            var dtoOut = refreshed.ToDto();
            // Not persisted fields: echo input for client convenience
            dtoOut.Title = dto.Title;
            dtoOut.Description = dto.Description;
            dtoOut.EstimatedAmount = dto.EstimatedAmount;
            dtoOut.Lines = dto.Lines;

            return ServiceResult<WorkOrderDto>.Success(dtoOut, "Work Order updated");
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkOrderDto>.Failure($"Error updating Work Order: {ex.Message}");
        }
    }

    public async Task<IServiceResult<WorkOrderDto>> SubmitAsync(SubmitWorkOrderDto dto, CancellationToken ct = default)
    {
        try
        {
            var entity = await LoadTrackedAsync(dto.WorkOrderId, ct);
            if (entity == null) return ServiceResult<WorkOrderDto>.Failure("Work Order not found");

            if (!EditableStates.Contains(entity.Status ?? string.Empty))
            {
                return ServiceResult<WorkOrderDto>.Failure("Only DRAFT or REVISED Work Orders can be submitted");
            }

            entity.Status = "AWAITING_APPROVAL";
            entity.Updatedat = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);

            var dtoOut = (await _unitOfWork.WorkOrderRepository.GetByIdAsync(dto.WorkOrderId, ct) ?? entity).ToDto();
            return ServiceResult<WorkOrderDto>.Success(dtoOut, "Submitted for approval");
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkOrderDto>.Failure($"Error submitting Work Order: {ex.Message}");
        }
    }

    public async Task<IServiceResult<WorkOrderDto>> ApproveAsync(ApproveWorkOrderDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto.WorkOrderId == Guid.Empty)
            {
                return ServiceResult<WorkOrderDto>.Failure("WorkOrderId is required");
            }

            var entity = await LoadTrackedAsync(dto.WorkOrderId, ct);
            if (entity == null) return ServiceResult<WorkOrderDto>.Failure("Work Order not found");

            if (!string.Equals(entity.Status, "AWAITING_APPROVAL", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<WorkOrderDto>.Failure("Only AWAITING_APPROVAL Work Orders can be approved");
            }

            entity.Status = "APPROVED";
            entity.Approvedat = DateTime.UtcNow;
            entity.Updatedat = entity.Approvedat;
            if (!string.IsNullOrWhiteSpace(dto.Note))
            {
                entity.Note = dto.Note;
            }

            await _unitOfWork.SaveChangesAsync(ct);

            var dtoOut = (await _unitOfWork.WorkOrderRepository.GetByIdAsync(dto.WorkOrderId, ct) ?? entity).ToDto();
            return ServiceResult<WorkOrderDto>.Success(dtoOut, "Work Order approved");
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkOrderDto>.Failure($"Error approving Work Order: {ex.Message}");
        }
    }

    public async Task<IServiceResult<WorkOrderDto>> RejectAsync(RejectWorkOrderDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto.WorkOrderId == Guid.Empty)
            {
                return ServiceResult<WorkOrderDto>.Failure("WorkOrderId is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                return ServiceResult<WorkOrderDto>.Failure("Reject reason is required");
            }

            var entity = await LoadTrackedAsync(dto.WorkOrderId, ct);
            if (entity == null) return ServiceResult<WorkOrderDto>.Failure("Work Order not found");

            if (!string.Equals(entity.Status, "AWAITING_APPROVAL", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<WorkOrderDto>.Failure("Only AWAITING_APPROVAL Work Orders can be rejected");
            }

            entity.Status = "REJECTED";
            entity.Note = dto.Reason; // Using Note to store reason (no dedicated field)
            entity.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(ct);

            var dtoOut = (await _unitOfWork.WorkOrderRepository.GetByIdAsync(dto.WorkOrderId, ct) ?? entity).ToDto();
            return ServiceResult<WorkOrderDto>.Success(dtoOut, "Work Order rejected");
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkOrderDto>.Failure($"Error rejecting Work Order: {ex.Message}");
        }
    }

    public async Task<IServiceResult<WorkOrderDto>> ReviseAsync(ReviseWorkOrderDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto.WorkOrderId == Guid.Empty)
            {
                return ServiceResult<WorkOrderDto>.Failure("WorkOrderId is required");
            }

            var entity = await LoadTrackedAsync(dto.WorkOrderId, ct);
            if (entity == null) return ServiceResult<WorkOrderDto>.Failure("Work Order not found");

            if (!string.Equals(entity.Status, "REJECTED", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<WorkOrderDto>.Failure("Only REJECTED Work Orders can be revised");
            }

            entity.Status = "REVISED";
            entity.UpdateFromDto(dto.Payload);
            await _unitOfWork.SaveChangesAsync(ct);

            var refreshed = await _unitOfWork.WorkOrderRepository.GetByIdAsync(dto.WorkOrderId, ct) ?? entity;
            var dtoOut = refreshed.ToDto();
            dtoOut.Title = dto.Payload.Title;
            dtoOut.Description = dto.Payload.Description;
            dtoOut.EstimatedAmount = dto.Payload.EstimatedAmount;
            dtoOut.Lines = dto.Payload.Lines;

            return ServiceResult<WorkOrderDto>.Success(dtoOut, "Work Order revised");
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkOrderDto>.Failure($"Error revising Work Order: {ex.Message}");
        }
    }

    public async Task<IServiceResult<WorkOrderDto>> StartAsync(StartWorkDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto.WorkOrderId == Guid.Empty)
            {
                return ServiceResult<WorkOrderDto>.Failure("WorkOrderId is required");
            }

            var entity = await LoadTrackedAsync(dto.WorkOrderId, ct);
            if (entity == null) return ServiceResult<WorkOrderDto>.Failure("Work Order not found");

            if (!string.Equals(entity.Status, "APPROVED", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<WorkOrderDto>.Failure("Only APPROVED Work Orders can be started");
            }

            entity.Status = "IN_PROGRESS";
            entity.Updatedat = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);

            var dtoOut = (await _unitOfWork.WorkOrderRepository.GetByIdAsync(dto.WorkOrderId, ct) ?? entity).ToDto();
            dtoOut.StartedAt = DateTime.UtcNow; // not persisted
            return ServiceResult<WorkOrderDto>.Success(dtoOut, "Work started");
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkOrderDto>.Failure($"Error starting Work Order: {ex.Message}");
        }
    }

    public async Task<IServiceResult<WorkOrderDto>> CompleteAsync(CompleteWorkDto dto, CancellationToken ct = default)
    {
        try
        {
            if (dto.WorkOrderId == Guid.Empty)
            {
                return ServiceResult<WorkOrderDto>.Failure("WorkOrderId is required");
            }

            var entity = await LoadTrackedAsync(dto.WorkOrderId, ct);
            if (entity == null) return ServiceResult<WorkOrderDto>.Failure("Work Order not found");

            if (!string.Equals(entity.Status, "IN_PROGRESS", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<WorkOrderDto>.Failure("Only IN_PROGRESS Work Orders can be completed");
            }

            entity.Status = "COMPLETED";
            entity.Updatedat = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);

            var dtoOut = (await _unitOfWork.WorkOrderRepository.GetByIdAsync(dto.WorkOrderId, ct) ?? entity).ToDto();
            dtoOut.CompletedAt = DateTime.UtcNow; // not persisted
            return ServiceResult<WorkOrderDto>.Success(dtoOut, "Work completed");
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkOrderDto>.Failure($"Error completing Work Order: {ex.Message}");
        }
    }

    public async Task<IServiceResult<WorkOrderDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _unitOfWork.WorkOrderRepository.GetByIdAsync(id, ct);
            if (entity == null) return ServiceResult<WorkOrderDto>.Failure("Work Order not found");
            return ServiceResult<WorkOrderDto>.Success(entity.ToDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkOrderDto>.Failure($"Error retrieving Work Order: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<WorkOrderDto>>> GetRangeAsync(Guid? centerId, DateOnly? date, string? status, Guid? technicianId, CancellationToken ct = default)
    {
        try
        {
            var entities = await _unitOfWork.WorkOrderRepository.GetRangeAsync(centerId, date, status, technicianId, ct);
            return ServiceResult<List<WorkOrderDto>>.Success(entities.ToDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<WorkOrderDto>>.Failure($"Error retrieving Work Orders: {ex.Message}");
        }
    }

    private async Task<Workorderapprovalthaontt?> LoadTrackedAsync(Guid id, CancellationToken ct)
    {
        return await _unitOfWork.WorkOrderRepository
            .GetAllQueryable()
            .Include(wo => wo.Order)
                .ThenInclude(o => o.Booking)
                    .ThenInclude(b => b.Serviceintakethaontt)
            .FirstOrDefaultAsync(wo => wo.WoaId == id, ct);
    }
}
