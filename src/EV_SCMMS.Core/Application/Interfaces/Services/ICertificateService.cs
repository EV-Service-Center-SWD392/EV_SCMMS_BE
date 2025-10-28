using EV_SCMMS.Core.Application.DTOs.Certificate;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Service interface for certificate management
/// </summary>
public interface ICertificateService
{
    Task<IServiceResult<CertificateDto>> CreateAsync(CreateCertificateDto dto);
    Task<IServiceResult<CertificateDto>> UpdateAsync(Guid id, UpdateCertificateDto dto);
    Task<IServiceResult<bool>> DeleteAsync(Guid id);
    Task<IServiceResult<CertificateDto>> GetByIdAsync(Guid id);
    Task<IServiceResult<List<CertificateDto>>> GetAllAsync();
    Task<IServiceResult<List<CertificateDto>>> GetActiveCertificatesAsync();
}