

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// User repository interface extending generic repository
/// </summary>
public interface IUserRepository : IGenericRepository<Object>
{
    // Add user-specific repository methods here
    // Example:
    Task<Object?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Object>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
}
