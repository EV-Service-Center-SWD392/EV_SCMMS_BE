using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories
{
    public class VehicleConditionDungVmRepository : GenericRepository<VehicleConditionDungVm>, IVehicleConditionDungVmRepository
    {
        public VehicleConditionDungVmRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<VehicleConditionDungVm>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default)
            => await _dbSet
                .Where(vc => vc.VehicleId == vehicleId && vc.IsActive == true)
                .ToListAsync(cancellationToken);
        


    }
}
