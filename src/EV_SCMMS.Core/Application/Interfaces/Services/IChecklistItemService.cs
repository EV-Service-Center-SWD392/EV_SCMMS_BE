using System;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.DTOs.ChecklistItems;
using EV_SCMMS.Core.Application.DTOs.Common;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface IChecklistItemService
{
    Task<PagedResult<ChecklistItemDto>> GetAsync(string? q, string status, int page, int pageSize, string sort, string order, CancellationToken ct);
    Task<ChecklistItemDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<ChecklistItemDto> CreateAsync(CreateChecklistItemDto dto, Guid actorId, CancellationToken ct);
    Task<ChecklistItemDto> UpdateAsync(Guid id, UpdateChecklistItemDto dto, Guid actorId, CancellationToken ct);
    Task<bool> SoftDeleteAsync(Guid id, Guid actorId, CancellationToken ct);
    Task<ChecklistItemDto> SetActiveAsync(Guid id, bool isActive, Guid actorId, CancellationToken ct);
}

