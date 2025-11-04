using EV_SCMMS.Core.Application.DTOs.Common;
using EV_SCMMS.Core.Application.DTOs.Vehicle;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Domain.Models;

public interface IVehicleService
{

  Task<IServiceResult<PagedResult<VehicleDto>>> GetListVehiclesAsync(VehicleQueryDto query, Guid customerId);
  Task<IServiceResult<VehicleDetailDto>> GetVehicleAsync(Guid id, Guid customerId);
  Task<IServiceResult<Vehicle>> CreateAsync(CreateVehicleDto dto, Guid customerId, CancellationToken ct = default);
  Task<IServiceResult<Vehicle>> UpdateAsync(Guid id, UpdateVehicleDto dto, Guid customerId, CancellationToken ct = default);
  Task<IServiceResult<Vehicle>> DeleteAsync(Guid id, Guid customerId, CancellationToken ct = default);

  Task<IServiceResult<List<Vehiclemodel>>> GetListVehicleModel();
}
