using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository abstraction for Workorderapprovalthaontt aggregate.
/// </summary>
public interface IWorkOrderRepository : IGenericRepository<Workorderapprovalthaontt>
{
    Task<Workorderapprovalthaontt?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Workorderapprovalthaontt?> GetByIntakeAsync(Guid intakeId, CancellationToken ct = default);
    Task<bool> ExistsActiveByIntakeAsync(Guid intakeId, CancellationToken ct = default);
    Task<List<Workorderapprovalthaontt>> GetRangeAsync(
        Guid? centerId,
        DateOnly? date,
        string? status,
        Guid? technicianId,
        CancellationToken ct = default);
}

