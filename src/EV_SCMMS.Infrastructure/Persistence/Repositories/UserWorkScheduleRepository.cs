using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Userworkscheduletuantm entity
/// </summary>
public class UserWorkScheduleRepository : GenericRepository<Userworkscheduletuantm>, IUserWorkScheduleRepository
{
    public UserWorkScheduleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Userworkscheduletuantm>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Userworkscheduletuantms
            .Where(x => x.Userid == userId && x.Isactive == true)
            .Include(x => x.Workschedule)
            .OrderBy(x => x.Workschedule.Starttime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Userworkscheduletuantm>> GetByWorkScheduleIdAsync(Guid workScheduleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Userworkscheduletuantms
            .Where(x => x.Workscheduleid == workScheduleId && x.Isactive == true)
            .Include(x => x.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Userworkscheduletuantm>> GetByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Userworkscheduletuantms
            .Where(x => x.Userid == userId && x.Isactive == true)
            .Include(x => x.Workschedule)
            .Where(x => x.Workschedule.Starttime >= startDate && x.Workschedule.Endtime <= endDate)
            .OrderBy(x => x.Workschedule.Starttime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUserAvailableAsync(Guid userId, Guid workScheduleId, CancellationToken cancellationToken = default)
    {
        var workSchedule = await _dbSet.Workscheduletuantms
            .FirstOrDefaultAsync(x => x.Workscheduleid == workScheduleId, cancellationToken);

        if (workSchedule == null) return false;

        var conflictingSchedules = await _dbSet.Userworkscheduletuantms
            .Where(x => x.Userid == userId && x.Isactive == true)
            .Include(x => x.Workschedule)
            .Where(x => x.Workschedule.Starttime < workSchedule.Endtime && 
                       x.Workschedule.Endtime > workSchedule.Starttime)
            .AnyAsync(cancellationToken);

        return !conflictingSchedules;
    }

    public async Task<List<Userworkscheduletuantm>> GetConflictingAssignmentsAsync(Guid userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Userworkscheduletuantms
            .Where(x => x.Userid == userId && x.Isactive == true)
            .Include(x => x.Workschedule)
            .Where(x => x.Workschedule.Starttime < endTime && x.Workschedule.Endtime > startTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> BulkAssignAsync(List<Userworkscheduletuantm> assignments, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbSet.AddRangeAsync(assignments, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Userworkscheduletuantm>> GetTechnicianWorkloadAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _dbSet.Userworkscheduletuantms
            .Where(x => x.Userid == userId && x.Isactive == true)
            .Include(x => x.Workschedule)
            .Where(x => x.Workschedule.Starttime >= startOfDay && x.Workschedule.Starttime < endOfDay)
            .OrderBy(x => x.Workschedule.Starttime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Userworkscheduletuantm>> GetAllWithUserAndScheduleAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Userworkscheduletuantms
            .Where(x => x.Isactive == true)
            .Include(x => x.User)
            .Include(x => x.Workschedule)
            .ThenInclude(x => x.Center)
            .OrderBy(x => x.User.Firstname)
            .ThenBy(x => x.Workschedule.Starttime)
            .ToListAsync(cancellationToken);
    }
}