using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Role entity
/// </summary>
public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get role by name
    /// </summary>
    /// <param name="roleName">Role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role entity if found</returns>
    public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Role>()
            .Where(r => r.Name.ToLower() == roleName.ToLower())
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Get all active roles
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active roles</returns>
    public async Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Role>()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get default user role
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Default user role</returns>
    public async Task<Role?> GetDefaultUserRoleAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Role>()
            .Where(r => r.Name.ToLower() == "customer")
            .FirstOrDefaultAsync(cancellationToken);
    }
}