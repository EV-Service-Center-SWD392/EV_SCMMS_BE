using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

public interface IChecklistItemRepository
{
    IQueryable<Checklistitemthaontt> Query();
    Task<bool> CodeExistsAsync(string code, Guid? excludeId, CancellationToken ct = default);
    Task AddAsync(Checklistitemthaontt entity, CancellationToken ct = default);
    Task UpdateAsync(Checklistitemthaontt entity, CancellationToken ct = default);
    Task<Checklistitemthaontt?> FindByIdAsync(Guid id, CancellationToken ct = default);
}

