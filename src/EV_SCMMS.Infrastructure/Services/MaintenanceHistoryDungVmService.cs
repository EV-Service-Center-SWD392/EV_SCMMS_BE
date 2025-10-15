using EV_SCMMS.Core.Application.DTOs.MaintenanceHistoryDungVmDto;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Infrastructure.Services
{
    public class MaintenanceHistoryDungVmService : IMaintenanceHistoryDungVmService
    {
        public Task<IServiceResult<MaintenanceHistoryDungVmDto>> CreateAsync(MaintenanceHistoryDungVmDto maintenanceHistoryDto)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult<bool>> DeleteAsync(Guid maintenanceHistoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult<List<MaintenanceHistoryDungVmDto>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult<MaintenanceHistoryDungVmDto>> GetByIdAsync(Guid maintenanceHistoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult<List<MaintenanceHistoryDungVmDto>>> GetByVehicleIdAsync(Guid vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult<MaintenanceHistoryDungVmDto>> UpdateAsync(Guid maintenanceHistoryId, MaintenanceHistoryDungVmDto maintenanceHistoryDto)
        {
            throw new NotImplementedException();
        }
    }
}
