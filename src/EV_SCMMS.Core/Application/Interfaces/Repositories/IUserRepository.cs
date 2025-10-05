using EV_SCMMS.Core.Domain.Entities;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// User repository interface extending generic repository
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    // Add user-specific repository methods here
    // Example:
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
}
