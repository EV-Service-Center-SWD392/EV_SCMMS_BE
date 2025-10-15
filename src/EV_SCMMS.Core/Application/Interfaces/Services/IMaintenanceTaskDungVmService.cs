using EV_SCMMS.Core.Application.DTOs.MaintenanceTaskDungVmDto;
using EV_SCMMS.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.Interfaces.Services
{
    /// <summary>
    /// Maintenance Task service interface
    /// </summary>
    public interface IMaintenanceTaskDungVmService
    {

        /// <summary>
        /// Gets all MaintenanceTask.
        /// </summary>
        /// <returns>Maintenance Task Dto</returns>
        Task<IServiceResult<List<MaintenanceTaskDungVmDto>>> GetAllAsync();

        /// <summary>
        /// Gets the by identifier MaintenanceTask.
        /// </summary>
        /// <param name="maintenanceTaskId">The maintenance task identifier.</param>
        /// <returns>Maintenance Task Dto by Id</returns>
        Task<IServiceResult<MaintenanceTaskDungVmDto>> GetByIdAsync(Guid maintenanceTaskId);

        /// <summary>
        /// Creates the MaintenanceTask.
        /// </summary>
        /// <param name="maintenanceTaskDto">The maintenance task dto.</param>
        /// <returns>Create Maintenance Task</returns>
        Task<IServiceResult<MaintenanceTaskDungVmDto>> CreateAsync(MaintenanceTaskDungVmCreateDto maintenanceTaskDto);

        /// <summary>
        /// Updates the MaintenanceTask.
        /// </summary>
        /// <param name="maintenanceTaskId">The maintenance task identifier.</param>
        /// <param name="maintenanceTaskDto">The maintenance task dto.</param>
        /// <returns>Update Maintenance Task</returns>
        Task<IServiceResult<MaintenanceTaskDungVmDto>> UpdateAsync(Guid maintenanceTaskId, MaintenanceTaskDungVmUpdateDto maintenanceTaskDto);

        /// <summary>
        /// Deletes the Maintenance Task.
        /// </summary>
        /// <param name="maintenanceTaskId">The maintenance task identifier.</param>
        /// <returns>Delete result</returns>
        Task<IServiceResult<bool>> DeleteAsync(Guid maintenanceTaskId);


        /// <summary>
        /// Gets the by order service identifier asynchronous.
        /// </summary>
        /// <param name="orderServiceId">The order service identifier.</param>
        /// <returns>Maintenance Task list with Order Service Id</returns>
        Task<IServiceResult<List<MaintenanceTaskDungVmDto>>> GetByOrderServiceIdAsync(int orderServiceId);

    }
}
