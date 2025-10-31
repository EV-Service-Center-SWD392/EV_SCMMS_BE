using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

public class ReceiptRepository : GenericRepository<Receiptcuongtq>, IReceiptRepository
{
    private readonly AppDbContext _context;
    public ReceiptRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Receiptcuongtq>> GetAllWithItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Receiptcuongtqs
            .Include(r => r.Receiptitemcuongtqs)
            .Include(r => r.Customer)
            .ToListAsync(cancellationToken);
    }

    public async Task<Receiptcuongtq?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Receiptcuongtqs
            .Include(r => r.Receiptitemcuongtqs)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Receiptid == id, cancellationToken);
    }

    public async Task<List<Receiptcuongtq>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Receiptcuongtqs
            .Where(r => r.Customerid == customerId)
            //.Include(r => r.Receiptitemcuongtqs)
            .Include(r => r.Customer)
            .ToListAsync(cancellationToken);
    }
}
