using EV_SCMMS.Core.Application.DTOs.Assignment;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Service interface for assignment operations
/// </summary>
public interface IAssignmentService
{
    Task<IServiceResult<AssignmentDto>> CreateAsync(CreateAssignmentDto dto, CancellationToken ct = default);
    Task<IServiceResult<AssignmentDto>> RescheduleAsync(Guid id, RescheduleAssignmentDto dto, CancellationToken ct = default);
    Task<IServiceResult<AssignmentDto>> ReassignAsync(Guid id, ReassignTechnicianDto dto, CancellationToken ct = default);
    Task<IServiceResult<CancelAssignmentResponseDto>> CancelAsync(Guid id, CancellationToken ct = default);
    Task<IServiceResult<bool>> NoShowAsync(Guid id, CancellationToken ct = default);
    Task<IServiceResult<List<AssignmentDto>>> GetRangeAsync(Guid? centerId, DateOnly? date, string? status, CancellationToken ct = default);
    Task<IServiceResult<AssignmentDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
}
