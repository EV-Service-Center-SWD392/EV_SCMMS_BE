using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Role entity operations
/// </summary>
public interface IRoleRepository : IGenericRepository<Role>
{
    /// <summary>
    /// Get role by name
    /// </summary>
    /// <param name="roleName">Role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role entity if found</returns>
    Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active roles
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active roles</returns>
    Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get default user role
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Default user role</returns>
    Task<Role?> GetDefaultUserRoleAsync(CancellationToken cancellationToken = default);
}