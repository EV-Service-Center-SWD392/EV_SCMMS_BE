using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for SpareparttypeTuht entity
/// </summary>
public class SparepartTypeRepository : GenericRepository<SpareparttypeTuht>, ISparepartTypeRepository
{
    public SparepartTypeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<SpareparttypeTuht?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Isactive == true)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<SpareparttypeTuht>> GetActiveTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpareparttypeTuht>> GetActiveSparepartTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpareparttypeTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        // Assuming status is related to IsActive boolean
        bool isActive = status.ToLower() == "active";
        return await _dbSet
            .Where(x => x.Isactive == isActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(x => x.Name == name && x.Isactive == true);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Typeid != excludeId.Value);
        }
        
        return await query.AnyAsync(cancellationToken);
    }
}