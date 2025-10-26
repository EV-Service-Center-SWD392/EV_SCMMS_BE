using EV_SCMMS.Core.Application.DTOs.ServiceIntake;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Service contract for Service Intake lifecycle management
/// </summary>
public interface IServiceIntakeService
{
    Task<IServiceResult<ServiceIntakeDto>> CreateAsync(CreateServiceIntakeDto dto, CancellationToken ct = default);
    Task<IServiceResult<ServiceIntakeDto>> UpdateAsync(Guid id, UpdateServiceIntakeDto dto, CancellationToken ct = default);
    Task<IServiceResult<ServiceIntakeDto>> VerifyAsync(Guid id, CancellationToken ct = default);
    Task<IServiceResult<ServiceIntakeDto>> FinalizeAsync(Guid id, CancellationToken ct = default);
    Task<IServiceResult<ServiceIntakeDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IServiceResult<List<ServiceIntakeDto>>> GetRangeAsync(Guid? centerId, DateOnly? date, string? status, Guid? technicianId, CancellationToken ct = default);
    Task<IServiceResult<bool>> CancelAsync(Guid id, CancellationToken ct = default);
}
