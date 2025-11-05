using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;
using EV_SCMMS.Infrastructure.Persistence.Repositories;

public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
{
  public VehicleRepository(AppDbContext context) : base(context)
  {
  }
}
