using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

public class OrderRepository : GenericRepository<Orderthaontt>, IOrderRepository
{
    private readonly AppDbContext _context;
    public OrderRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Orderthaontt?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orderthaontts
            .Include(o => o.Orderservicethaontts)
                .ThenInclude(s => s.Service)
            .Include(o => o.Orderspareparts)
                .ThenInclude(p => p.Sparepart)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Orderid == id, cancellationToken);
    }
}
