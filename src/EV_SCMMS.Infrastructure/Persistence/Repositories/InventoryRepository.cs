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

    public async Task<IEnumerable<InventoryTuht>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.InventoryTuhts.AsNoTracking().Where(x => x.Centerid == centerId && x.Isactive == true).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryTuht>> GetActiveInventoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.InventoryTuhts.AsNoTracking().Where(x => x.Isactive == true).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryTuht>> GetLowStockInventoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.InventoryTuhts.AsNoTracking().Where(x => x.Isactive == true && x.Quantity <= x.Minimumstocklevel).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.InventoryTuhts.AsNoTracking().Where(x => x.Isactive == true && x.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryTuht>> GetAllWithProperties(CancellationToken cancellationToken = default)
    {
        return await _dbSet.InventoryTuhts.AsNoTracking()
            .Include(x => x.Center)
            .ToListAsync(cancellationToken);
    }

    public Task SoftDeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}