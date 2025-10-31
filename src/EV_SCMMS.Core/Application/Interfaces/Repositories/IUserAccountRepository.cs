using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserAccount entity operations
/// </summary>
public interface IUserAccountRepository : IGenericRepository<Useraccount>
{
    Task<List<Useraccount>> GetByRoleAsync(string role, CancellationToken cancellationToken = default);
    Task<Useraccount?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<Useraccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> UpdateRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}