using EV_SCMMS.Core.Application.DTOs.VehicleConditionDungVm;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Infrastructure.Services
{
    public class VehicleConditionDungVmService : IVehicleConditionDungVmService
    {
        public Task<IServiceResult<VehicleConditionDungVmDto>> CreateAsync(VehicleConditionDungVmDto vehicleConditionDto)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult<bool>> DeleteAsync(Guid vehicleConditionId)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult<List<VehicleConditionDungVmDto>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult<VehicleConditionDungVmDto>> GetByIdAsync(Guid vehicleConditionId)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult<List<VehicleConditionDungVmDto>>> GetByVehicleIdAsync(Guid vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult<VehicleConditionDungVmDto>> UpdateAsync(Guid vehicleConditionId, VehicleConditionDungVmDto vehicleConditionDto)
        {
            throw new NotImplementedException();
        }
    }
}
