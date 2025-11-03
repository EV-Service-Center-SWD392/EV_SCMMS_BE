using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

public class UserRoleRepository : GenericRepository<Userrole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Userrole>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Userroles
            .Where(x => x.Isactive == true)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Userrole?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Userroles
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}