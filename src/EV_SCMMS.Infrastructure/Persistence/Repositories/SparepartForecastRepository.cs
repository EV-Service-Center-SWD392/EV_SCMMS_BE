using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for SparepartforecastTuht entity
/// </summary>
public class SparepartForecastRepository : GenericRepository<SparepartforecastTuht>, ISparepartForecastRepository
{
    public SparepartForecastRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SparepartforecastTuht>> GetBySparepartIdAsync(Guid sparepartId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Sparepartid == sparepartId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartforecastTuht>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Sparepart)
            .ThenInclude(s => s!.Inventory)
            .Where(x => x.Sparepart!.Inventory!.Centerid == centerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartforecastTuht>> GetByForecastedByAsync(string forecastedBy, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Forecastedby == forecastedBy)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartforecastTuht>> GetByApprovedByAsync(Guid approvedBy, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Approvedby == approvedBy)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartforecastTuht>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Forecastdate >= startDate && x.Forecastdate <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartforecastTuht>> GetActiveForecastsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Status == "Active")
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartforecastTuht>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Status == "Pending")
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartforecastTuht>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SparepartforecastTuht>> GetLowReorderPointForecastsAsync(CancellationToken cancellationToken = default)
    {
        // Get forecasts where reorder point is low (you might need to adjust the logic)
        return await _dbSet
            .Where(x => x.Reorderpoint < 10) // Example: reorder point less than 10
            .ToListAsync(cancellationToken);
    }
}