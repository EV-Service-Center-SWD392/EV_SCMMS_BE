using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

public class TransactionRepository : GenericRepository<Transactioncuongtq>, ITransactionRepository
{
  public TransactionRepository(AppDbContext context) : base(context)
  {
  }

  public async Task<List<Transactioncuongtq>> GetAllByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
  {
    return await _dbSet.Transactioncuongtqs
        .Where(t => t.Orderid == orderId)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<Transactioncuongtq>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    return await _dbSet.Transactioncuongtqs
        .Where(t => t.Order.Customerid == userId)
        .ToListAsync(cancellationToken);
  }

  public async Task<Transactioncuongtq?> GetByIdIncludeOrderAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _dbSet.Transactioncuongtqs
       .Where(t => t.Transactionid == id)
       .Include(t => t.Order)
       .FirstOrDefaultAsync(cancellationToken);
  }

  public async Task<Transactioncuongtq?> GetByPaymentIdAsync(int paymentId, CancellationToken cancellationToken = default)
  {
    return await _dbSet.Transactioncuongtqs
            .Where(t => t.Paymentid == paymentId)
            .FirstOrDefaultAsync(cancellationToken);
  }
}
