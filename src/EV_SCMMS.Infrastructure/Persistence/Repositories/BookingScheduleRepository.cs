using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories
{
    public class BookingScheduleRepository : GenericRepository<Bookingschedule>, IBookingScheduleRepository
    {
        public BookingScheduleRepository(AppDbContext context) : base(context)
        {
        }
    }
}