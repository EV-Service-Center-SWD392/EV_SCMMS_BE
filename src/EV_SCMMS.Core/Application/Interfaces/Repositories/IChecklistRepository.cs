using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories
{
    public interface IChecklistRepository
    {
        Task<List<Checklistitemthaontt>> GetItemsAsync(CancellationToken ct = default);

        Task<List<Checklistresponsethaontt>> GetResponsesAsync(Guid intakeId, CancellationToken ct = default);

        Task UpsertResponsesAsync(
            Guid intakeId,
            IEnumerable<(Guid ItemId, string? Value, string? Note, string? PhotoUrl)> responses,
            CancellationToken ct = default);
    }
}
