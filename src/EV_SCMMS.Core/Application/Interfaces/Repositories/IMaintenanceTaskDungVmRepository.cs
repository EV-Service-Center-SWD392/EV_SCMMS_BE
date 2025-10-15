using EV_SCMMS.Core.Application.DTOs.MaintenanceTaskDungVmDto;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories
{
    public interface IMaintenanceTaskDungVmRepository : IGenericRepository<MaintenanceTaskDungVm>
    {
        Task<IEnumerable<MaintenanceTaskDungVm>> GetByOrderServiceIdAsync(int OrderServiceId, CancellationToken ck = default);
    }
}
