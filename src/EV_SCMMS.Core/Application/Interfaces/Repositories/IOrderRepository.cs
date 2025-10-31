using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Orderthaontt>
{
    // Get order with services and spareparts included
    Task<Orderthaontt?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
}
