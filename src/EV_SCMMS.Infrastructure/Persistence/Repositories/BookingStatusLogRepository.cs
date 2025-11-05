using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;
using EV_SCMMS.Infrastructure.Persistence.Repositories;

public class BookingStatusLogRepository : GenericRepository<Bookingstatusloghuykt>, IBookingStatusLogRepository
{
  public BookingStatusLogRepository(AppDbContext context) : base(context)
  {
  }
}
