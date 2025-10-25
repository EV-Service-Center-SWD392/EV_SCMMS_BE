using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for WorkScheduleTuantm entity
/// </summary>
public class WorkScheduleRepository : GenericRepository<WorkScheduleTuantm>, IWorkScheduleRepository
{
    public WorkScheduleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<WorkScheduleTuantm>> GetByTechnicianIdAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Set<WorkScheduleTuantm>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.TechnicianId == technicianId)
            .OrderBy(x => x.WorkDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkScheduleTuantm>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, Guid? technicianId = default, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Set<WorkScheduleTuantm>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.WorkDate >= startDate && x.WorkDate <= endDate);

        if (technicianId.HasValue)
        {
            query = query.Where(x => x.TechnicianId == technicianId);
        }

        return await query
            .OrderBy(x => x.WorkDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkScheduleTuantm>> GetAvailableTechniciansAsync(DateOnly workDate, TimeOnly startTime, TimeOnly endTime, CancellationToken cancellationToken = default)
    {
        // base time range for the day
        var slotStart = workDate.ToDateTime(startTime);
        var slotEnd = workDate.ToDateTime(endTime);

        // Get all active schedules overlapping the requested time window
        var schedules = _dbSet.Set<WorkScheduleTuantm>()
            .AsNoTracking()
            .Where(s => s.IsActive && s.WorkDate == workDate && s.StartTime < endTime && s.EndTime > startTime);

        // Compute current assignment counts and filter schedules with remaining capacity
        var result = await schedules
            .Select(s => new
            {
                Schedule = s,
                AssignedCount = _dbSet.Assignmentthaontts
                    .Where(a => a.Isactive == true && a.Technicianid == s.TechnicianId)
                    .Where(a => a.Plannedstartutc < slotEnd && a.Plannedendutc > slotStart)
                    .Count()
            })
            .Where(x => x.Schedule.SlotCapacity > x.AssignedCount)
            .Select(x => x.Schedule)
            .OrderBy(x => x.TechnicianId)
            .ThenBy(x => x.StartTime)
            .ToListAsync(cancellationToken);

        return result;
    }
}

