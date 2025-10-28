using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories
{
    public interface IServiceIntakeRepository : IGenericRepository<Serviceintakethaontt>
    {
        Task<Serviceintakethaontt?> GetByIdWithIncludesAsync(Guid id, CancellationToken ct = default);

        Task<List<Serviceintakethaontt>> GetRangeAsync(
            Guid? centerId,
            DateOnly? date,
            string? status,
            Guid? technicianId,
            CancellationToken ct = default);
    }
}
