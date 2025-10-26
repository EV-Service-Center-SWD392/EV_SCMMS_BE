using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Workorderapprovalthaontt aggregate.
/// All read operations use AsNoTracking.
/// </summary>
public class WorkOrderRepository : GenericRepository<Workorderapprovalthaontt>, IWorkOrderRepository
{
    private static readonly string[] ActiveStatuses = new[] { "DRAFT", "AWAITING_APPROVAL", "APPROVED", "IN_PROGRESS" };

    public WorkOrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Workorderapprovalthaontt?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet.Workorderapprovalthaontts
            .AsNoTracking()
            .Include(wo => wo.Order)
                .ThenInclude(o => o.Booking)
                    .ThenInclude(b => b.Serviceintakethaontt)
            .FirstOrDefaultAsync(wo => wo.WoaId == id, ct);
    }

    public async Task<Workorderapprovalthaontt?> GetByIntakeAsync(Guid intakeId, CancellationToken ct = default)
    {
        return await _dbSet.Workorderapprovalthaontts
            .AsNoTracking()
            .Include(wo => wo.Order)
                .ThenInclude(o => o.Booking)
                    .ThenInclude(b => b.Serviceintakethaontt)
            .FirstOrDefaultAsync(wo =>
                wo.Order.Booking != null &&
                wo.Order.Booking.Serviceintakethaontt != null &&
                wo.Order.Booking.Serviceintakethaontt.Intakeid == intakeId,
                ct);
    }

    public async Task<bool> ExistsActiveByIntakeAsync(Guid intakeId, CancellationToken ct = default)
    {
        return await _dbSet.Workorderapprovalthaontts
            .AsNoTracking()
            .Include(wo => wo.Order)
                .ThenInclude(o => o.Booking)
                    .ThenInclude(b => b.Serviceintakethaontt)
            .AnyAsync(wo =>
                wo.Order.Booking != null &&
                wo.Order.Booking.Serviceintakethaontt != null &&
                wo.Order.Booking.Serviceintakethaontt.Intakeid == intakeId &&
                wo.Status != null && ActiveStatuses.Contains(wo.Status, StringComparer.OrdinalIgnoreCase),
                ct);
    }

    public async Task<List<Workorderapprovalthaontt>> GetRangeAsync(Guid? centerId, DateOnly? date, string? status, Guid? technicianId, CancellationToken ct = default)
    {
        var query = _dbSet.Workorderapprovalthaontts
            .AsNoTracking()
            .Include(wo => wo.Order)
                .ThenInclude(o => o.Booking)
                    .ThenInclude(b => b.Slot)
            .Include(wo => wo.Order)
                .ThenInclude(o => o.Booking)
                    .ThenInclude(b => b.Serviceintakethaontt)
            .Where(wo => wo.Isactive != false);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(wo => wo.Status != null && wo.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        if (centerId.HasValue)
        {
            var cid = centerId.Value;
            query = query.Where(wo => wo.Order.Booking != null && wo.Order.Booking.Slot != null && wo.Order.Booking.Slot.Centerid == cid);
        }

        if (technicianId.HasValue)
        {
            var tid = technicianId.Value;
            query = query.Where(wo => wo.Order.Booking != null && wo.Order.Booking.Serviceintakethaontt != null && wo.Order.Booking.Serviceintakethaontt.Advisorid == tid);
        }

        if (date.HasValue)
        {
            var expected = date.Value;
            query = query.Where(wo => wo.Createdat.HasValue && DateOnly.FromDateTime(DateTime.SpecifyKind(wo.Createdat.Value, DateTimeKind.Utc)) == expected);
        }

        return await query
            .OrderByDescending(wo => wo.Createdat)
            .ToListAsync(ct);
    }
}

