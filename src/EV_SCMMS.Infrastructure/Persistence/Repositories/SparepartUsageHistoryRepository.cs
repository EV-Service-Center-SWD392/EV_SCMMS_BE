using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for SparepartusagehistoryTuht entity
/// </summary>
public class SparepartUsageHistoryRepository : GenericRepository<SparepartusagehistoryTuht>, ISparepartUsageHistoryRepository
{
    public SparepartUsageHistoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SparepartusagehistoryTuht>> GetBySparepartIdAsync(Guid sparepartId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Sparepartid == sparepartId && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartusagehistoryTuht>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Sparepart)
            .ThenInclude(s => s!.Inventory)
            .Where(x => x.Sparepart!.Inventory!.Centerid == centerId && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartusagehistoryTuht>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Useddate >= startDate && x.Useddate <= endDate && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartusagehistoryTuht>> GetActiveHistoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartusagehistoryTuht>> GetActiveUsageHistoryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalUsageAsync(Guid sparepartId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(x => x.Sparepartid == sparepartId && x.Isactive == true);

        if (startDate.HasValue)
            query = query.Where(x => x.Useddate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(x => x.Useddate <= endDate.Value);

        return await query.SumAsync(x => x.Quantityused, cancellationToken);
    }

    public async Task<IEnumerable<SparepartusagehistoryTuht>> GetUsageStatisticsAsync(Guid? centerId = null, Guid? sparepartId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(x => x.Sparepart)
            .ThenInclude(s => s!.Inventory)
            .Where(x => x.Isactive == true);

        if (centerId.HasValue)
            query = query.Where(x => x.Sparepart!.Inventory!.Centerid == centerId.Value);

        if (sparepartId.HasValue)
            query = query.Where(x => x.Sparepartid == sparepartId.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartusagehistoryTuht>> GetByUsageTypeAsync(string usageType, CancellationToken cancellationToken = default)
    {
        // Note: The entity doesn't have Purpose field, using Status instead
        return await _dbSet
            .Where(x => x.Status == usageType && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartusagehistoryTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        // Assuming status is related to IsActive boolean  
        bool isActive = status.ToLower() == "active";
        return await _dbSet
            .Where(x => x.Isactive == isActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartusagehistoryTuht>> GetByVehicleIdAsync(string vehicleId, CancellationToken cancellationToken = default)
    {
        // Note: The entity doesn't have vehicleId field, this is a placeholder implementation
        // You might need to adjust this based on your actual entity structure
        return await _dbSet
            .Where(x => x.Isactive == true)
            .ToListAsync(cancellationToken);
    }
}