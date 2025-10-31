using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

public class ReceiptItemRepository : GenericRepository<Receiptitemcuongtq>, IReceiptItemRepository
{
    public ReceiptItemRepository(AppDbContext context) : base(context)
    {
    }
}
