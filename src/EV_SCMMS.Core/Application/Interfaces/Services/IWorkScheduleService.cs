using EV_SCMMS.Core.Application.DTOs.WorkSchedule;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Service interface for managing technician work schedules and queue
/// </summary>
public interface IWorkScheduleService
{
    // CRUD
    Task<IServiceResult<WorkScheduleDto>> CreateAsync(CreateWorkScheduleDto dto);
    Task<IServiceResult<WorkScheduleDto>> UpdateAsync(Guid id, UpdateWorkScheduleDto dto);
    Task<IServiceResult<bool>> DeleteAsync(Guid id);
    Task<IServiceResult<WorkScheduleDto>> GetByIdAsync(Guid id);

    // Queries
    Task<IServiceResult<List<WorkScheduleDto>>> GetByTechnicianIdAsync(Guid technicianId);
    Task<IServiceResult<List<WorkScheduleDto>>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, Guid? technicianId = default);
    Task<IServiceResult<List<WorkScheduleDto>>> GetAvailableTechniciansAsync(DateOnly workDate, TimeOnly startTime, TimeOnly endTime);
}

