using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Sparepartreplenishmentrequest entity
/// </summary>
public class SparepartReplenishmentRequestRepository : GenericRepository<Sparepartreplenishmentrequest>, ISparepartReplenishmentRequestRepository
{
    public SparepartReplenishmentRequestRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Sparepartreplenishmentrequest>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Sparepartreplenishmentrequests
            .AsNoTracking()
            .Include(x => x.Sparepart)
            .ThenInclude(s => s!.Inventory)
            .Where(x => x.Sparepart!.Inventory!.Centerid == centerId && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sparepartreplenishmentrequest>> GetBySparepartIdAsync(Guid sparepartId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Sparepartreplenishmentrequests
            .Where(x => x.Sparepartid == sparepartId && x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sparepartreplenishmentrequest>> GetByForecastIdAsync(Guid forecastId, CancellationToken cancellationToken = default)
    {
        // Assuming there's a relationship or foreign key - implementing as placeholder
        return await _dbSet.Sparepartreplenishmentrequests
            .Where(x => x.Forecastid == forecastId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sparepartreplenishmentrequest>> GetByApprovedByAsync(Guid approvedBy, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Sparepartreplenishmentrequests
            .Where(x => x.Approvedby == approvedBy)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sparepartreplenishmentrequest>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Sparepartreplenishmentrequests
            .Where(x => x.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sparepartreplenishmentrequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Sparepartreplenishmentrequests
            .Where(x => x.Createdat >= startDate && x.Createdat <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sparepartreplenishmentrequest>> GetActiveRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Sparepartreplenishmentrequests
            .Where(x => x.Isactive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sparepartreplenishmentrequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Sparepartreplenishmentrequests
            .Where(x => x.Status == "PENDING")
            .ToListAsync(cancellationToken);
    }

    public Task SoftDeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}