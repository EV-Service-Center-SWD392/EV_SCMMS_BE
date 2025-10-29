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
        return await _dbSet.SpareparttypeTuhts
            .Where(x => x.Isactive == true)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<SpareparttypeTuht>> GetActiveTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.SpareparttypeTuhts
            .Where(x => x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpareparttypeTuht>> GetActiveSparepartTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.SpareparttypeTuhts
            .Where(x => x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SpareparttypeTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        // Assuming status is related to IsActive boolean
        return await _dbSet.SpareparttypeTuhts
            .Where(x => x.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.SpareparttypeTuhts.Where(x => x.Name == name);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Typeid != excludeId.Value);
        }
        
        return await query.AnyAsync(cancellationToken);
    }

    public Task SoftDeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}