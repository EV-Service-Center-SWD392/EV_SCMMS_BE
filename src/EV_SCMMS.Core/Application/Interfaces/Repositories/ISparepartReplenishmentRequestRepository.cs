using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Sparepartreplenishmentrequest entity operations
/// </summary>
public interface ISparepartReplenishmentRequestRepository : IGenericRepository<Sparepartreplenishmentrequest>
{
    Task<IEnumerable<Sparepartreplenishmentrequest>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sparepartreplenishmentrequest>> GetBySparepartIdAsync(Guid sparepartId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sparepartreplenishmentrequest>> GetByForecastIdAsync(Guid forecastId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sparepartreplenishmentrequest>> GetByApprovedByAsync(Guid approvedBy, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sparepartreplenishmentrequest>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sparepartreplenishmentrequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Sparepartreplenishmentrequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id);
}