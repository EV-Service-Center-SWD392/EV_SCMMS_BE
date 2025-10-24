using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories
{
    public class BookingRepository : GenericRepository<Bookinghuykt>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context)
        {
        }
    }
}