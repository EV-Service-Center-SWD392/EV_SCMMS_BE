using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Workscheduletuantm entity
/// </summary>
public class WorkScheduleRepository : GenericRepository<Workscheduletuantm>, IWorkScheduleRepository
{
    public WorkScheduleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Workscheduletuantm>> GetByCenterIdAsync(Guid centerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Workscheduletuantms
            .Where(x => x.Centerid == centerId && x.Isactive == true)
            .Include(x => x.Center)
            .OrderBy(x => x.Starttime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Workscheduletuantm>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? centerId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Workscheduletuantms
            .Where(x => x.Isactive == true && x.Starttime >= startDate && x.Endtime <= endDate);

        if (centerId.HasValue)
        {
            query = query.Where(x => x.Centerid == centerId);
        }

        return await query
            .Include(x => x.Center)
            .OrderBy(x => x.Starttime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Workscheduletuantm>> GetActiveSchedulesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Workscheduletuantms
            .Where(x => x.Isactive == true)
            .Include(x => x.Center)
            .OrderBy(x => x.Starttime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Workscheduletuantm>> GetByTechnicianIdAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Workscheduletuantms
            .Where(x => x.Isactive == true)
            .Include(x => x.Userworkscheduletuantms.Where(u => u.Userid == technicianId && u.Isactive == true))
            .Where(x => x.Userworkscheduletuantms.Any(u => u.Userid == technicianId && u.Isactive == true))
            .OrderBy(x => x.Starttime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Workscheduletuantm>> GetSchedulesWithAvailableCapacityAsync(DateTime startTime, DateTime endTime, Guid? centerId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Workscheduletuantms
            .Where(x => x.Isactive == true && x.Starttime < endTime && x.Endtime > startTime);

        if (centerId.HasValue)
        {
            query = query.Where(x => x.Centerid == centerId);
        }

        return await query
            .Include(x => x.Userworkscheduletuantms)
            .OrderBy(x => x.Starttime)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetAssignedTechnicianCountAsync(Guid workScheduleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Userworkscheduletuantms
            .Where(x => x.Workscheduleid == workScheduleId && x.Isactive == true)
            .CountAsync(cancellationToken);
    }
}

