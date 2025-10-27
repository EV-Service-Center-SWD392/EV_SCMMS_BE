using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Center entity operations
/// </summary>
public interface ICenterRepository : IGenericRepository<Centertuantm>
{
    Task<IEnumerable<Centertuantm>> GetActiveCentersAsync(CancellationToken cancellationToken = default);
    Task<Centertuantm?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id);
}