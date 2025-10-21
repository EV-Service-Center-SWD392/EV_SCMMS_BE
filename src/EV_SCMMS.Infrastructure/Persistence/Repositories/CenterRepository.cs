
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Center entity
/// </summary>
public class CenterRepository : GenericRepository<Centertuantm>, ICenterRepository
{
    public CenterRepository(AppDbContext context) : base(context)
    {
    }


    public async Task<IEnumerable<Centertuantm>> GetActiveCentersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Centertuantms
            .Where(x => x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public Task SoftDeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}