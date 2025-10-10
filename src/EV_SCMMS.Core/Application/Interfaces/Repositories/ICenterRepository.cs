using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Center entity operations
/// </summary>
public interface ICenterRepository : IGenericRepository<Center>
{
    Task<IEnumerable<Center>> GetActiveCentersAsync(CancellationToken cancellationToken = default);
}