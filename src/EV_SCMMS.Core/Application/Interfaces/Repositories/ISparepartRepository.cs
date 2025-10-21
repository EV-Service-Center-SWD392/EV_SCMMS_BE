using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for SparepartTuht entity operations
/// </summary>
public interface ISparepartRepository : IGenericRepository<SparepartTuht>
{
    Task<SparepartTuht?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartTuht>> GetByTypeIdAsync(Guid typeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartTuht>> GetByInventoryIdAsync(Guid inventoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartTuht>> GetByVehicleModelIdAsync(int vehicleModelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartTuht>> GetByManufacturerAsync(string manufacturer, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartTuht>> GetActiveSparepartsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<SparepartTuht>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    Task<bool> IsNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id);
}