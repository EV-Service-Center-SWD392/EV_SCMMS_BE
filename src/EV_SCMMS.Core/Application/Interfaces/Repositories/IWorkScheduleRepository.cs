using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Workscheduletuantm entity operations
/// </summary>
public interface IWorkScheduleRepository : IGenericRepository<Workscheduletuantm>
{
    Task<List<Workscheduletuantm>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default);
    Task<List<Workscheduletuantm>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? centerId = null, CancellationToken cancellationToken = default);
    Task<List<Workscheduletuantm>> GetActiveSchedulesAsync(CancellationToken cancellationToken = default);
    Task<List<Workscheduletuantm>> GetByTechnicianIdAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<List<Workscheduletuantm>> GetSchedulesWithAvailableCapacityAsync(DateTime startTime, DateTime endTime, Guid? centerId = null, CancellationToken cancellationToken = default);
    Task<int> GetAssignedTechnicianCountAsync(Guid workScheduleId, CancellationToken cancellationToken = default);
    Task<Workscheduletuantm?> GetExistingScheduleAsync(Guid centerId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
}

