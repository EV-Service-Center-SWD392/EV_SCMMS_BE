using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Certificate entity operations
/// </summary>
public interface ICertificateRepository : IGenericRepository<Certificatetuantm>
{
    Task<List<Certificatetuantm>> GetActiveCertificatesAsync(CancellationToken cancellationToken = default);
    Task<Certificatetuantm?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}