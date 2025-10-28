using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories
{
    public interface IBookingRepository : IGenericRepository<Bookinghuykt>
    {
        Task<Bookinghuykt?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Bookinghuykt>> GetPendingAsync(Guid? centerId, DateOnly? date, CancellationToken ct = default);
        Task<bool> ExistsApprovedOverlapAsync(Guid centerId, DateTime startUtc, DateTime endUtc, CancellationToken ct = default);
    }
}
