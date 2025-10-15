using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories
{
    public class MaintenanceHistoryDungVmRepository : GenericRepository<MaintenanceHistoryDungVm>, IMaintenanceHistoryDungVmRepository
    {
        public MaintenanceHistoryDungVmRepository(AppDbContext dbContext) : base(dbContext) { }



    }

}
