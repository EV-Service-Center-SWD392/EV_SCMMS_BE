using EV_SCMMS.Core.Application.DTOs.WorkOrder;
using EV_SCMMS.Core.Application.Results;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface IWorkOrderService
{
    Task<IServiceResult<WorkOrderDto>> CreateAsync(CreateWorkOrderDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> UpdateAsync(Guid id, UpdateWorkOrderDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> SubmitAsync(SubmitWorkOrderDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> ApproveAsync(ApproveWorkOrderDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> RejectAsync(RejectWorkOrderDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> ReviseAsync(ReviseWorkOrderDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> StartAsync(StartWorkDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> CompleteAsync(CompleteWorkDto dto, CancellationToken ct = default);
    Task<IServiceResult<WorkOrderDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IServiceResult<List<WorkOrderDto>>> GetRangeAsync(Guid? centerId, DateOnly? date, string? status, Guid? technicianId, CancellationToken ct = default);
}

