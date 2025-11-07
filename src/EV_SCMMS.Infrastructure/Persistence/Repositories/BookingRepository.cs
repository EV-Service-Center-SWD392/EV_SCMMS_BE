using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories
{
    public class BookingRepository : GenericRepository<Bookinghuykt>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Bookinghuykt?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbSet.Bookinghuykts
                .AsNoTracking()
                .Include(b => b.Slot)
                .FirstOrDefaultAsync(b => b.Bookingid == id, ct);
        }

        public async Task<List<Bookinghuykt>> GetPendingAsync(Guid? centerId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default)
        {
            var query = _dbSet.Bookinghuykts
                .AsNoTracking()
                .Include(b => b.Slot)
                .Where(b => b.Status == BookingStatusConstant.Pending || b.Status == BookingStatusConstant.Requested);

            if (centerId.HasValue)
            {
                var center = centerId.Value;
                query = query.Where(b => b.Slot != null && b.Slot.Centerid == center);
            }

            if (startDate.HasValue)
            {
                query = query.Where(b => b.Slot != null && b.BookingDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(b => b.Slot != null && b.BookingDate <= endDate);
            }



            return await query
                .OrderByDescending(x => x.BookingDate)
                .ThenByDescending(x => x.Slot.Startutc)
                .ToListAsync(ct);
        }

        // ! Deprecated

        public async Task<bool> ExistsApprovedOverlapAsync(Guid centerId, DateTime startUtc, DateTime endUtc, CancellationToken ct = default)
        {
            var overlapping = await _dbSet.Bookinghuykts
                .AsNoTracking()
                .Include(b => b.Slot)
                .Where(b => b.Status == "APPROVED")
                .Where(b => b.Slot != null && b.Slot.Centerid == centerId)
                // .Where(b => b.Slot!.Startutc < endUtc && startUtc < b.Slot.Endutc)
                .ToListAsync(ct);

            if (overlapping.Count == 0)
            {
                return false;
            }

            var slotCapacity = overlapping
                .Select(b => b.Slot?.Capacity)
                .Where(c => c.HasValue && c.Value > 0)
                .Select(c => c!.Value)
                .DefaultIfEmpty(1)
                .Max();

            return overlapping.Count >= slotCapacity;
        }
    }
}
