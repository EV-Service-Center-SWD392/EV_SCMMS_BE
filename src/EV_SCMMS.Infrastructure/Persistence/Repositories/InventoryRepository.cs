using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for InventoryTuht entity
/// </summary>
public class InventoryRepository : GenericRepository<InventoryTuht>, IInventoryRepository
{
    public InventoryRepository(AppDbContext context) : base(context)
    {
    }

    // DO NOT OVERRIDE GetPagedAsync - use base GenericRepository implementation

    // Override AddAsync to set default values
    public override async Task<InventoryTuht> AddAsync(InventoryTuht entity, CancellationToken cancellationToken = default)
    {
        entity.Createdat = DateTime.UtcNow;
        entity.Isactive = true;
        
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Override UpdateAsync to set updated date
    public override async Task UpdateAsync(InventoryTuht entity, CancellationToken cancellationToken = default)
    {
        entity.Updatedat = DateTime.UtcNow;
        
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // Override SoftDeleteAsync with proper soft delete logic
    public override async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            entity.Isactive = false;
            entity.Updatedat = DateTime.UtcNow;
            
            await UpdateAsync(entity, cancellationToken);
        }
    }

    public async Task<IEnumerable<InventoryTuht>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(x => x.Centerid == centerId && x.Isactive == true).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryTuht>> GetActiveInventoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(x => x.Isactive == true).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryTuht>> GetLowStockInventoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(x => x.Isactive == true && x.Quantity <= x.Minimumstocklevel).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(x => x.Isactive == true && x.Status == status).ToListAsync(cancellationToken);
    }
}