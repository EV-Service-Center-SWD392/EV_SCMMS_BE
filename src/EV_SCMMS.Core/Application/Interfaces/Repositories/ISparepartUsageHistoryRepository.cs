using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for SparepartusagehistoryTuht entity operations
/// </summary>
public interface ISparepartUsageHistoryRepository : IGenericRepository<SparepartusagehistoryTuht>
{
    Task<IEnumerable<SparepartusagehistoryTuht>> GetBySparepartIdAsync(Guid sparepartId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartusagehistoryTuht>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartusagehistoryTuht>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartusagehistoryTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartusagehistoryTuht>> GetActiveUsageHistoryAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalUsageAsync(Guid sparepartId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartusagehistoryTuht>> GetUsageStatisticsAsync(Guid? centerId = null, Guid? sparepartId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartusagehistoryTuht>> GetByVehicleIdAsync(string vehicleId, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id);
}