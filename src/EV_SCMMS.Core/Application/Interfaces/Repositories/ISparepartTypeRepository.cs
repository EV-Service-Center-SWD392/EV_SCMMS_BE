using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for SpareparttypeTuht entity operations
/// </summary>
public interface ISparepartTypeRepository : IGenericRepository<SpareparttypeTuht>
{
    Task<SpareparttypeTuht?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<SpareparttypeTuht>> GetActiveSparepartTypesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SpareparttypeTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<bool> IsNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
}