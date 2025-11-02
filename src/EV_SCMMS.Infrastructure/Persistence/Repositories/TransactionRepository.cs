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
        .Include(t => t.Receiptcuongtqs)
        .Include(t=> t.Paymentmethod)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<Transactioncuongtq>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    return await _dbSet.Transactioncuongtqs
        .Where(t => t.Order.Customerid == userId)
        .Include(t => t.Paymentmethod)
        .ToListAsync(cancellationToken);
  }

  public async Task<Transactioncuongtq?> GetByIdIncludeOrderAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _dbSet.Transactioncuongtqs
       .Where(t => t.Transactionid == id)
       .Include(t => t.Order)
       .Include(t => t.Receiptcuongtqs)
       .Include(t => t.Paymentmethod)
       .FirstOrDefaultAsync(cancellationToken);
  }

  public Task<Transactioncuongtq?> GetByOrderCodeAsync(int orderCode, CancellationToken cancellationToken = default)
  {
    return _dbSet.Transactioncuongtqs
        .Where(t => t.Paymentid == orderCode)
        .Include(t => t.Paymentmethod)
        .FirstOrDefaultAsync(cancellationToken);
  }

  public async Task<Transactioncuongtq?> GetByPaymentIdAsync(int paymentId, CancellationToken cancellationToken = default)
  {
    return await _dbSet.Transactioncuongtqs
            .Where(t => t.Paymentid == paymentId)
            .Include(t => t.Receiptcuongtqs)
            .FirstOrDefaultAsync(cancellationToken);
  }
}
