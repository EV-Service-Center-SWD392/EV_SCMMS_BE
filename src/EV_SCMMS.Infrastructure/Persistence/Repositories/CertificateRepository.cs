using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Certificate entity
/// </summary>
public class CertificateRepository : GenericRepository<Certificatetuantm>, ICertificateRepository
{
    public CertificateRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Certificatetuantm>> GetActiveCertificatesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.Isactive == true)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Certificatetuantm?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}