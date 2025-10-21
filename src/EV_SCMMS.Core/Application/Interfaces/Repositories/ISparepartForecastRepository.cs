using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for SparepartforecastTuht entity operations
/// </summary>
public interface ISparepartForecastRepository : IGenericRepository<SparepartforecastTuht>
{
    Task<IEnumerable<SparepartforecastTuht>> GetBySparepartIdAsync(Guid sparepartId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartforecastTuht>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartforecastTuht>> GetByForecastedByAsync(string forecastedBy, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartforecastTuht>> GetByApprovedByAsync(Guid approvedBy, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartforecastTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartforecastTuht>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartforecastTuht>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartforecastTuht>> GetLowReorderPointForecastsAsync(CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id);
}