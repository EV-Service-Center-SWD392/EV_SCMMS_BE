using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserCertificate entity operations
/// </summary>
public interface IUserCertificateRepository : IGenericRepository<Usercertificatetuantm>
{
    Task<List<Usercertificatetuantm>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Usercertificatetuantm>> GetByCertificateIdAsync(Guid certificateId, CancellationToken cancellationToken = default);
    Task<List<Usercertificatetuantm>> GetExpiringCertificatesAsync(int daysAhead = 30, CancellationToken cancellationToken = default);
    Task<bool> HasCertificateAsync(Guid userId, Guid certificateId, CancellationToken cancellationToken = default);
}