using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Assignmentthaontt entity
/// </summary>
public class AssignmentRepository : GenericRepository<Assignmentthaontt>, IAssignmentRepository
{
    public AssignmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<int> CountAssignmentsByTechnicianAndRangeAsync(
        Guid technicianId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct = default)
    {
        // Overlap rule: (AStart < BEnd) && (BStart < AEnd)
        return await _dbSet.Assignmentthaontts
            .AsNoTracking()
            .Where(a => a.Isactive == true && a.Technicianid == technicianId)
            .Where(a => a.Plannedstartutc != null && a.Plannedendutc != null)
            .Where(a => a.Plannedstartutc! < endUtc && startUtc < a.Plannedendutc!)
            .CountAsync(ct);
    }

    public async Task<bool> ExistsOverlapAsync(
        Guid technicianId,
        DateTime startUtc,
        DateTime endUtc,
        Guid? excludeAssignmentId = null,
        CancellationToken ct = default)
    {
        var query = _dbSet.Assignmentthaontts
            .AsNoTracking()
            .Where(a => a.Isactive == true && a.Technicianid == technicianId)
            .Where(a => a.Plannedstartutc != null && a.Plannedendutc != null)
            .Where(a => a.Plannedstartutc! < endUtc && startUtc < a.Plannedendutc!);

        if (excludeAssignmentId.HasValue)
        {
            query = query.Where(a => a.Assignmentid != excludeAssignmentId.Value);
        }

        return await query.AnyAsync(ct);
    }

    public async Task<bool> ExistsOverlapWithStatusesAsync(
        Guid technicianId,
        DateTime startUtc,
        DateTime endUtc,
        IEnumerable<string> statuses,
        Guid? excludeAssignmentId = null,
        CancellationToken ct = default)
    {
        var statusSet = new HashSet<string>(statuses ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        var query = _dbSet.Assignmentthaontts
            .AsNoTracking()
            .Where(a => a.Isactive == true && a.Technicianid == technicianId)
            .Where(a => a.Status != null && statusSet.Contains(a.Status))
            .Where(a => a.Plannedstartutc != null && a.Plannedendutc != null)
            .Where(a => a.Plannedstartutc! < endUtc && startUtc < a.Plannedendutc!);

        if (excludeAssignmentId.HasValue)
        {
            query = query.Where(a => a.Assignmentid != excludeAssignmentId.Value);
        }

        return await query.AnyAsync(ct);
    }

    public async Task<int> CountAssignmentsWithStatusesByTechnicianAndRangeAsync(
        Guid technicianId,
        DateTime startUtc,
        DateTime endUtc,
        IEnumerable<string> statuses,
        CancellationToken ct = default)
    {
        var statusSet = new HashSet<string>(statuses ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        return await _dbSet.Assignmentthaontts
            .AsNoTracking()
            .Where(a => a.Isactive == true && a.Technicianid == technicianId)
            .Where(a => a.Status != null && statusSet.Contains(a.Status))
            .Where(a => a.Plannedstartutc != null && a.Plannedendutc != null)
            .Where(a => a.Plannedstartutc! < endUtc && startUtc < a.Plannedendutc!)
            .CountAsync(ct);
    }

    public async Task<List<Assignmentthaontt>> GetRangeAsync(
        Guid? centerId,
        DateOnly? date,
        string? status,
        CancellationToken ct = default)
    {
        // Include booking and slot to allow reading CenterId
        var query = _dbSet.Assignmentthaontts
            .AsNoTracking()
            .Include(a => a.Booking)
                .ThenInclude(b => b.Slot)
            .Where(a => a.Isactive == true);

        if (centerId.HasValue)
        {
            // only assignments whose booking links to a slot at this center
            query = query.Where(a => a.Booking.Slot != null && a.Booking.Slot.Centerid == centerId.Value);
        }

        if (date.HasValue)
        {
            query = query.Where(a => a.Plannedstartutc.HasValue && DateOnly.FromDateTime(a.Plannedstartutc.Value) == date.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status != null && a.Status == status);
        }

        // Use projection to avoid selecting 'note' column which doesn't exist in DB
        var results = await query
            .Select(a => new
            {
                a.Assignmentid,
                a.Bookingid,
                a.Technicianid,
                a.Plannedstartutc,
                a.Plannedendutc,
                a.Queueno,
                a.Status,
                a.Isactive,
                a.Createdat,
                a.Updatedat,
                a.Booking
            })
            .OrderBy(a => a.Plannedstartutc)
            .ThenBy(a => a.Technicianid)
            .ToListAsync(ct);

        // Map back to entity
        return results.Select(r => new Assignmentthaontt
        {
            Assignmentid = r.Assignmentid,
            Bookingid = r.Bookingid,
            Technicianid = r.Technicianid,
            Plannedstartutc = r.Plannedstartutc,
            Plannedendutc = r.Plannedendutc,
            Queueno = r.Queueno,
            Status = r.Status,
            Note = null,
            Isactive = r.Isactive,
            Createdat = r.Createdat,
            Updatedat = r.Updatedat,
            Booking = r.Booking
        }).ToList();
    }
}
