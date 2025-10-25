using EV_SCMMS.Core.Application.DTOs.WorkSchedule;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Business logic for technician work schedules and availability
/// </summary>
public class WorkScheduleService : IWorkScheduleService
{
    private readonly IUnitOfWork _unitOfWork;

    public WorkScheduleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<WorkScheduleDto>> CreateAsync(CreateWorkScheduleDto dto)
    {
        try
        {
            // basic validation
            if (dto.EndTime <= dto.StartTime)
            {
                return ServiceResult<WorkScheduleDto>.Failure("EndTime must be after StartTime");
            }

            var entity = dto.ToEntity();
            await _unitOfWork.WorkScheduleRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<WorkScheduleDto>.Success(entity.ToDto(), "Work schedule created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkScheduleDto>.Failure($"Error creating work schedule: {ex.Message}");
        }
    }

    public async Task<IServiceResult<WorkScheduleDto>> UpdateAsync(Guid id, UpdateWorkScheduleDto dto)
    {
        try
        {
            var existing = await _unitOfWork.WorkScheduleRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ServiceResult<WorkScheduleDto>.Failure("Work schedule not found");
            }

            if (dto.EndTime <= dto.StartTime)
            {
                return ServiceResult<WorkScheduleDto>.Failure("EndTime must be after StartTime");
            }

            existing.UpdateFromDto(dto);
            await _unitOfWork.WorkScheduleRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<WorkScheduleDto>.Success(existing.ToDto(), "Work schedule updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkScheduleDto>.Failure($"Error updating work schedule: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var existing = await _unitOfWork.WorkScheduleRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ServiceResult<bool>.Failure("Work schedule not found");
            }

            // Soft delete: mark inactive
            existing.IsActive = false;
            existing.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.WorkScheduleRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Work schedule deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting work schedule: {ex.Message}");
        }
    }

    public async Task<IServiceResult<WorkScheduleDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _unitOfWork.WorkScheduleRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<WorkScheduleDto>.Failure("Work schedule not found");
            }

            return ServiceResult<WorkScheduleDto>.Success(entity.ToDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<WorkScheduleDto>.Failure($"Error retrieving work schedule: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<WorkScheduleDto>>> GetByTechnicianIdAsync(Guid technicianId)
    {
        try
        {
            var items = await _unitOfWork.WorkScheduleRepository.GetByTechnicianIdAsync(technicianId);
            return ServiceResult<List<WorkScheduleDto>>.Success(items.ToDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<WorkScheduleDto>>.Failure($"Error retrieving schedules: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<WorkScheduleDto>>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, Guid? technicianId = default)
    {
        try
        {
            var items = await _unitOfWork.WorkScheduleRepository.GetByDateRangeAsync(startDate, endDate, technicianId);
            return ServiceResult<List<WorkScheduleDto>>.Success(items.ToDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<WorkScheduleDto>>.Failure($"Error retrieving schedules by range: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<WorkScheduleDto>>> GetAvailableTechniciansAsync(DateOnly workDate, TimeOnly startTime, TimeOnly endTime)
    {
        try
        {
            var items = await _unitOfWork.WorkScheduleRepository.GetAvailableTechniciansAsync(workDate, startTime, endTime);
            return ServiceResult<List<WorkScheduleDto>>.Success(items.ToDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<WorkScheduleDto>>.Failure($"Error retrieving available technicians: {ex.Message}");
        }
    }
}

