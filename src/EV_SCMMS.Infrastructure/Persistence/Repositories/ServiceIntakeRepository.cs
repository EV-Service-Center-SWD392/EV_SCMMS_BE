using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Serviceintakethaontt aggregate
/// </summary>
public class ServiceIntakeRepository : GenericRepository<Serviceintakethaontt>, IServiceIntakeRepository
{
    public ServiceIntakeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Serviceintakethaontt?> GetByIdWithIncludesAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet.Serviceintakethaontts
            .AsNoTracking()
            .Include(si => si.Booking)
                .ThenInclude(b => b.Slot)
            .Include(si => si.Booking)
                .ThenInclude(b => b.Vehicle)
            .Include(si => si.Booking)
                .ThenInclude(b => b.Assignmentthaontts)
            .Include(si => si.Booking)
                .ThenInclude(b => b.Assignmentthaontts)
            .Include(si => si.Advisor)
            .FirstOrDefaultAsync(si => si.Intakeid == id, ct);
    }

    public async Task<List<Serviceintakethaontt>> GetRangeAsync(
        Guid? centerId,
        DateOnly? date,
        string? status,
        Guid? technicianId,
        CancellationToken ct = default)
    {
        var query = _dbSet.Serviceintakethaontts
            .AsNoTracking()
            .Include(si => si.Booking)
                .ThenInclude(b => b.Slot)
            .Include(si => si.Booking)
                .ThenInclude(b => b.Vehicle)
            .Include(si => si.Booking)
                .ThenInclude(b => b.Assignmentthaontts)
            .Where(si => si.Isactive != false);

        if (centerId.HasValue)
        {
            var center = centerId.Value;
            query = query.Where(si => si.Booking.Slot != null && si.Booking.Slot.Centerid == center);
        }

        if (date.HasValue)
        {
            var expectedDate = date.Value;
            query = query.Where(si =>
                si.Createdat.HasValue &&
                DateOnly.FromDateTime(DateTime.SpecifyKind(si.Createdat.Value, DateTimeKind.Utc)) == expectedDate);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(si => si.Status != null && si.Status!.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        if (technicianId.HasValue)
        {
            var techId = technicianId.Value;
            // Filter by assigned technician via Booking -> Assignmentthaontts
            query = query.Where(si => si.Booking.Assignmentthaontts.Any(a => a.Isactive == true && a.Technicianid == techId));
        }

        return await query
            .OrderByDescending(si => si.Createdat)
            .ToListAsync(ct);
    }
}
