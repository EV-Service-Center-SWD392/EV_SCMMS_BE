using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories
{
  public class PaymentMethodRepository : GenericRepository<Paymentmethodcuongtq>, IPaymentmethodrepository
  {
    private readonly AppDbContext _context;

    public PaymentMethodRepository(AppDbContext context) : base(context)
    {
      _context = context;
    }

    public async Task<Paymentmethodcuongtq?> GetByIdAsync(int id, CancellationToken ct = default)
    {
      return await _context.Paymentcuongtqs
          .Where (pm => pm.Paymentmethodid == id)
          .FirstOrDefaultAsync(pm => pm.Paymentmethodid == id, ct);
    }

  }
}
