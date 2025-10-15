using EV_SCMMS.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories
{
    public interface IOrderThaoNttRepository : IGenericRepository<OrderThaoNtt>
    {
        //Task<IEnumerable<OrderThaoNtt>> GetByIdAsync(int OrderId, CancellationToken cancellationToken = default);
    }
}
