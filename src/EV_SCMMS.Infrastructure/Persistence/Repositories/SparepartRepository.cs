using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for SparepartTuht entity
/// </summary>
public class SparepartRepository : GenericRepository<SparepartTuht>, ISparepartRepository
{
    public SparepartRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<SparepartTuht?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Isactive == true)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<SparepartTuht>> GetByTypeIdAsync(Guid typeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Typeid == typeId && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartTuht>> GetByInventoryIdAsync(Guid inventoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Inventoryid == inventoryId && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartTuht>> GetByVehicleModelIdAsync(int vehicleModelId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Vehiclemodelid == vehicleModelId && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartTuht>> GetByManufacturerAsync(string manufacturer, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Manufacture == manufacturer && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartTuht>> GetActiveSparepartsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        // Assuming status is related to Isactive boolean
        bool isActive = status.ToLower() == "active";
        return await _dbSet
            .Where(x => x.Isactive == isActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartTuht>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Unitprice >= minPrice && x.Unitprice <= maxPrice && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(x => x.Name == name && x.Isactive == true);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Sparepartid != excludeId.Value);
        }
        
        return await query.AnyAsync(cancellationToken);
    }
}