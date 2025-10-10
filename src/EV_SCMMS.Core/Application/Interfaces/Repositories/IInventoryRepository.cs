using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for InventoryTuht entity operations
/// </summary>
public interface IInventoryRepository : IGenericRepository<InventoryTuht>
{
    Task<IEnumerable<InventoryTuht>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryTuht>> GetActiveInventoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryTuht>> GetLowStockInventoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
}