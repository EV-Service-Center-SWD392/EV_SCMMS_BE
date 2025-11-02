using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories
{
    public interface IPaymentmethodrepository : IGenericRepository<Paymentmethodcuongtq>
    {
        Task<Paymentmethodcuongtq?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
