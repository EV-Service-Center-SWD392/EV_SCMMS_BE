using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for WorkScheduleTuantm entity operations
/// </summary>
public interface IWorkScheduleRepository : IGenericRepository<WorkScheduleTuantm>
{
    Task<List<WorkScheduleTuantm>> GetByTechnicianIdAsync(Guid technicianId, CancellationToken cancellationToken = default);

    Task<List<WorkScheduleTuantm>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, Guid? technicianId = default, CancellationToken cancellationToken = default);

    Task<List<WorkScheduleTuantm>> GetAvailableTechniciansAsync(DateOnly workDate, TimeOnly startTime, TimeOnly endTime, CancellationToken cancellationToken = default);
}

