using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

public interface IUserRoleRepository : IGenericRepository<Userrole>
{
    Task<List<Userrole>> GetActiveRolesAsync(CancellationToken cancellationToken = default);
    Task<Userrole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}