using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

public interface ITransactionRepository : IGenericRepository<Transactioncuongtq>
{
  Task<List<Transactioncuongtq>> GetAllByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

  Task<List<Transactioncuongtq>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

  Task<Transactioncuongtq?> GetByPaymentIdAsync(int paymentId, CancellationToken cancellationToken = default);

  Task<Transactioncuongtq?> GetByIdIncludeOrderAsync(Guid id, CancellationToken cancellationToken = default);

  Task<Transactioncuongtq?> GetByOrderCodeAsync(int orderCode, CancellationToken cancellationToken = default);
}
