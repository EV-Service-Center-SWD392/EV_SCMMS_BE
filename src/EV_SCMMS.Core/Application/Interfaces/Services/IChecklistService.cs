using EV_SCMMS.Core.Application.DTOs.ServiceIntake;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Service contract for EV checklist catalog and responses
/// </summary>
public interface IChecklistService
{
    Task<IServiceResult<List<ChecklistItemDto>>> GetItemsAsync(CancellationToken ct = default);
    Task<IServiceResult<List<ChecklistResponseItemDto>>> GetResponsesAsync(Guid intakeId, CancellationToken ct = default);
    Task<IServiceResult<bool>> UpsertResponsesAsync(UpsertChecklistResponsesDto dto, CancellationToken ct = default);
}
