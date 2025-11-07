using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

public class UserCertificateRepository : GenericRepository<Usercertificatetuantm>, IUserCertificateRepository
{
    public UserCertificateRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Usercertificatetuantm>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Usercertificatetuantms
            .Where(x => x.Userid == userId && x.Isactive == true)
            .Include(x => x.Certificate)
            .Include(x => x.User)
            .OrderBy(x => x.Certificate.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Usercertificatetuantm>> GetByCertificateIdAsync(Guid certificateId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Usercertificatetuantms
            .Where(x => x.Certificateid == certificateId && x.Isactive == true)
            .Include(x => x.Certificate)
            .Include(x => x.User)
            .OrderBy(x => x.User.Firstname)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Usercertificatetuantm>> GetExpiringCertificatesAsync(int daysAhead = 30, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);
        
        return await _dbSet.Usercertificatetuantms
            .Where(x => x.Isactive == true && x.Createdat.HasValue)
            .Include(x => x.Certificate)
            .Include(x => x.User)
            .Where(x => x.Createdat.Value.AddYears(1) <= cutoffDate)
            .OrderBy(x => x.Createdat.Value.AddYears(1))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasCertificateAsync(Guid userId, Guid certificateId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Usercertificatetuantms
            .AnyAsync(x => x.Userid == userId && x.Certificateid == certificateId && x.Isactive == true, cancellationToken);
    }

    public async Task<List<Usercertificatetuantm>> GetPendingCertificatesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Usercertificatetuantms
            .Where(x => x.Status == "PENDING" && x.Isactive == true)
            .Include(x => x.Certificate)
            .Include(x => x.User)
            .OrderBy(x => x.Createdat)
            .ToListAsync(cancellationToken);
    }
}