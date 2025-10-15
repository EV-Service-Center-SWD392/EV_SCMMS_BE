using EV_SCMMS.Core.Application.DTOs.MaintenanceTaskDungVmDto;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories
{
    public class MaintenanceTaskDungVmRepository : GenericRepository<MaintenanceTaskDungVm>, IMaintenanceTaskDungVmRepository
    {
        public MaintenanceTaskDungVmRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<MaintenanceTaskDungVm>> GetByOrderServiceIdAsync(int orderServiceId, CancellationToken cancellationToken = default)
            => await _dbSet
                .Include(m => m.OrderService)
                .Where(m => m.OrderServiceId == orderServiceId && m.IsActive == true)
                .ToListAsync(cancellationToken);


    }
}
