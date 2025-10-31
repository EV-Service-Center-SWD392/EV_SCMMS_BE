using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

public interface IReceiptRepository : IGenericRepository<Receiptcuongtq>
{
    Task<List<Receiptcuongtq>> GetAllWithItemsAsync(CancellationToken cancellationToken = default);
    Task<Receiptcuongtq?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Receiptcuongtq>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

}
