using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories
{
    public class VehicleConditionDungVmRepository : GenericRepository<VehicleConditionDungVm>, IVehicleConditionDungVmRepository
    {
        public VehicleConditionDungVmRepository(AppDbContext context) : base(context) { }


        
    }
}
