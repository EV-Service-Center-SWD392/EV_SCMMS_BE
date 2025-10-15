using EV_SCMMS.Core.Application.DTOs.VehicleConditionDungVm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.Interfaces.Services
{
    /// <summary>
    /// Vehicle Condition services interface
    /// </summary>
    public interface IVehicleConditionDungVmService
    {
        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<IServiceResult<List<VehicleConditionDungVmDto>>> GetAllAsync();

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="vehicleConditionId">The vehicle condition identifier.</param>
        /// <returns></returns>
        Task<IServiceResult<VehicleConditionDungVmDto>> GetByIdAsync(Guid vehicleConditionId);

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="vehicleConditionDto">The vehicle condition dto.</param>
        /// <returns>Create Vehicle Condition</returns>
        Task<IServiceResult<VehicleConditionDungVmDto>> CreateAsync(VehicleConditionDungVmDto vehicleConditionDto);

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="vehicleConditionId">The vehicle condition identifier.</param>
        /// <param name="vehicleConditionDto">The vehicle condition dto.</param>
        /// <returns>Update Vehicle Condition</returns>
        Task<IServiceResult<VehicleConditionDungVmDto>> UpdateAsync(Guid vehicleConditionId, VehicleConditionDungVmDto vehicleConditionDto);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="vehicleConditionId">The vehicle condition identifier.</param>
        /// <returns>Delete result</returns>
        Task<IServiceResult<bool>> DeleteAsync(Guid vehicleConditionId);

        /// <summary>
        /// Gets the by vehicle identifier asynchronous.
        /// </summary>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <returns></returns>
        Task<IServiceResult<List<VehicleConditionDungVmDto>>> GetByVehicleIdAsync(Guid vehicleId);
        
    }
}
