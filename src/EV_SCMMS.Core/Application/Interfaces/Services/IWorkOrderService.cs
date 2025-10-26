using EV_SCMMS.Core.Application.DTOs.WorkOrder;
using EV_SCMMS.Core.Application.Results;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface IWorkOrderService
{
    Task<IServiceResult<WorkOrderDto>> CreateAsync(CreateWorkOrderDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> UpdateAsync(Guid id, UpdateWorkOrderDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> SubmitAsync(Guid id, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> ApproveAsync(Guid id, ApproveWorkOrderDto dto, Guid approvedBy, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> RejectAsync(Guid id, RejectWorkOrderDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> ReviseAsync(Guid id, ReviseWorkOrderDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IServiceResult<List<WorkOrderDto>>> GetRangeAsync(Guid? centerId, DateOnly? date, string? status, Guid? technicianId, CancellationToken ct = default);
    Task<IServiceResult<List<WorkOrderDto>>> GetMyWorkOrdersAsync(Guid currentUserId, string role, CancellationToken ct);
}

