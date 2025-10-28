using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Userworkscheduletuantm entity operations
/// </summary>
public interface IUserWorkScheduleRepository : IGenericRepository<Userworkscheduletuantm>
{
    Task<List<Userworkscheduletuantm>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Userworkscheduletuantm>> GetByWorkScheduleIdAsync(Guid workScheduleId, CancellationToken cancellationToken = default);
    Task<List<Userworkscheduletuantm>> GetByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<bool> IsUserAvailableAsync(Guid userId, Guid workScheduleId, CancellationToken cancellationToken = default);
    Task<List<Userworkscheduletuantm>> GetConflictingAssignmentsAsync(Guid userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    Task<bool> BulkAssignAsync(List<Userworkscheduletuantm> assignments, CancellationToken cancellationToken = default);
    Task<List<Userworkscheduletuantm>> GetTechnicianWorkloadAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
}