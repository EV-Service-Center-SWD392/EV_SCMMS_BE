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
    private readonly AppDbContext _context;

    public ServiceIntakeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Serviceintakethaontt?> GetByIdWithIncludesAsync(Guid id, CancellationToken ct = default)
    {
        var intake = await _dbSet.Serviceintakethaontts
            .AsNoTracking()
            .Include(si => si.Booking)
                .ThenInclude(b => b.Slot)
            .Include(si => si.Booking)
                .ThenInclude(b => b.Vehicle)
            .Include(si => si.Advisor)
            .FirstOrDefaultAsync(si => si.Intakeid == id, ct);

        if (intake?.Booking != null)
        {
            // Load assignments separately to avoid querying note column
            var assignments = await _context.Set<Assignmentthaontt>()
                .AsNoTracking()
                .Where(a => a.Bookingid == intake.Booking.Bookingid)
                .Select(a => new Assignmentthaontt
                {
                    Assignmentid = a.Assignmentid,
                    Bookingid = a.Bookingid,
                    Technicianid = a.Technicianid,
                    Plannedstartutc = a.Plannedstartutc,
                    Plannedendutc = a.Plannedendutc,
                    Queueno = a.Queueno,
                    Status = a.Status,
                    Isactive = a.Isactive,
                    Createdat = a.Createdat,
                    Updatedat = a.Updatedat
                    // Explicitly exclude Note field
                })
                .ToListAsync(ct);

            intake.Booking.Assignmentthaontts = assignments;
        }

        return intake;
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
            var normalizedStatus = status.Trim().ToUpperInvariant();
            query = query.Where(si => si.Status != null && si.Status.ToUpper() == normalizedStatus);
        }

        if (technicianId.HasValue)
        {
            var techId = technicianId.Value;
            // Filter by checking if technician has any active assignment for this booking
            var bookingIdsWithTech = await _context.Set<Assignmentthaontt>()
                .AsNoTracking()
                .Where(a => a.Isactive == true && a.Technicianid == techId)
                .Select(a => a.Bookingid)
                .ToListAsync(ct);

            query = query.Where(si => bookingIdsWithTech.Contains(si.Bookingid));
        }

        var intakes = await query
            .OrderByDescending(si => si.Createdat)
            .ToListAsync(ct);

        // Load assignments separately for each intake
        foreach (var intake in intakes.Where(i => i.Booking != null))
        {
            var assignments = await _context.Set<Assignmentthaontt>()
                .AsNoTracking()
                .Where(a => a.Bookingid == intake.Booking!.Bookingid)
                .Select(a => new Assignmentthaontt
                {
                    Assignmentid = a.Assignmentid,
                    Bookingid = a.Bookingid,
                    Technicianid = a.Technicianid,
                    Plannedstartutc = a.Plannedstartutc,
                    Plannedendutc = a.Plannedendutc,
                    Queueno = a.Queueno,
                    Status = a.Status,
                    Isactive = a.Isactive,
                    Createdat = a.Createdat,
                    Updatedat = a.Updatedat
                })
                .ToListAsync(ct);

            intake.Booking.Assignmentthaontts = assignments;
        }

        return intakes;
    }
}
