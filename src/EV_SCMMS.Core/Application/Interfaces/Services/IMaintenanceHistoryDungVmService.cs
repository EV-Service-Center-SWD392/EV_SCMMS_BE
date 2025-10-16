using EV_SCMMS.Core.Application.DTOs.MaintenanceHistoryDungVmDto;
using EV_SCMMS.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.Interfaces.Services
{
    /// <summary>
    /// Interface for MaintenanceHistoryDungVm service
    /// </summary>
    public interface IMaintenanceHistoryDungVmService
    {
        /// <summary>
        /// Gets all Maintenance History.
        /// </summary>
        /// <returns>Maintenance History Dto</returns>
        Task<IServiceResult<List<MaintenanceHistoryDungVmDto>>> GetAllAsync();

        /// <summary>
        /// Gets the by Id Maintenance History.
        /// </summary>
        /// <param name="maintenanceHistoryId">The maintenance history identifier.</param>
        /// <returns></returns>
        Task<IServiceResult<MaintenanceHistoryDungVmDto>> GetByIdAsync(Guid maintenanceHistoryId);

        /// <summary>
        /// Creates the Maintenance History.
        /// </summary>
        /// <param name="maintenanceHistoryDto">The maintenance history dto.</param>
        /// <returns>Create Maintenance History</returns>
        Task<IServiceResult<MaintenanceHistoryDungVmDto>> CreateAsync(MaintenanceHistoryDungVmCreateDto maintenanceHistoryDto);

        /// <summary>
        /// Updates the Maintenance History.
        /// </summary>
        /// <param name="maintenanceHistoryId">The maintenance history identifier.</param>
        /// <param name="maintenanceHistoryDto">The maintenance history dto.</param>
        /// <returns>Update result</returns>
        Task<IServiceResult<MaintenanceHistoryDungVmDto>> UpdateAsync(Guid maintenanceHistoryId, MaintenanceHistoryDungVmUpdateDto maintenanceHistoryDto);

        /// <summary>
        /// Deletes the Maintenance History.
        /// </summary>
        /// <param name="maintenanceHistoryId">The maintenance history identifier.</param>
        /// <returns>Delete result<</returns>
        Task<IServiceResult<bool>> DeleteAsync(Guid maintenanceHistoryId);

        /// <summary>
        /// Gets Maintenance History by the  vehicle identifier .
        /// </summary>
        /// <param name="vehicleId">The vehicle Id.</param>
        /// <returns>Maintenance History of vehicle</returns>
        Task<IServiceResult<List<MaintenanceHistoryDungVmDto>>> GetByVehicleIdAsync(Guid vehicleId);

    }
}
