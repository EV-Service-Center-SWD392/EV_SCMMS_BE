using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for user-center membership (Usercentertuantm)
/// </summary>
public class UserCenterRepository : GenericRepository<Usercentertuantm>, IUserCenterRepository
{
    public UserCenterRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> IsUserInCenterAsync(Guid userId, Guid centerId, CancellationToken ct = default)
    {
        return await _dbSet.Set<Usercentertuantm>()
            .AsNoTracking()
            .AnyAsync(x => x.Userid == userId && x.Centerid == centerId && x.Isactive == true, ct);
    }
}

