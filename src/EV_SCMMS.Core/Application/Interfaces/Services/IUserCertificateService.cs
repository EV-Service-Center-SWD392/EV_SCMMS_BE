using EV_SCMMS.Core.Application.DTOs.UserCertificate;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface IUserCertificateService
{
    Task<IServiceResult<UserCertificateDto>> AssignCertificateAsync(AssignCertificateDto dto);
    Task<IServiceResult<bool>> RevokeCertificateAsync(Guid userCertificateId);
    Task<IServiceResult<List<UserCertificateDto>>> GetUserCertificatesAsync(Guid userId);
    Task<IServiceResult<List<UserCertificateDto>>> GetCertificateHoldersAsync(Guid certificateId);
    Task<IServiceResult<List<UserCertificateDto>>> GetExpiringCertificatesAsync(int daysAhead = 30);
    Task<IServiceResult<bool>> HasCertificateAsync(Guid userId, Guid certificateId);
}