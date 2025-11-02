using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;
using EV_SCMMS.Infrastructure.Persistence.Repositories;

public class VehicleModelRepository : GenericRepository<Vehiclemodel>, IVehicleModelRepository
{
  public VehicleModelRepository(AppDbContext context) : base(context)
  {
  }
}
