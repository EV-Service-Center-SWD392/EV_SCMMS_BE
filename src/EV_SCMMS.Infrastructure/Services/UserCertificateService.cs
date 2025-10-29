using EV_SCMMS.Core.Application.DTOs.UserCertificate;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Service implementation for user certificate management
/// </summary>
public class UserCertificateService : IUserCertificateService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserCertificateService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<UserCertificateDto>> AssignCertificateAsync(AssignCertificateDto dto)
    {
        try
        {
            // Check if user already has this certificate
            var hasExisting = await _unitOfWork.UserCertificateRepository.HasCertificateAsync(dto.UserId, dto.CertificateId);
            if (hasExisting)
            {
                return ServiceResult<UserCertificateDto>.Failure("User already has this certificate");
            }

            var entity = dto.ToEntity();
            await _unitOfWork.UserCertificateRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<UserCertificateDto>.Success(entity.ToDto(), "Certificate assigned successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<UserCertificateDto>.Failure($"Error assigning certificate: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> RevokeCertificateAsync(Guid userCertificateId)
    {
        try
        {
            var existing = await _unitOfWork.UserCertificateRepository.GetByIdAsync(userCertificateId);
            if (existing == null)
            {
                return ServiceResult<bool>.Failure("User certificate not found");
            }

            existing.Isactive = false;
            existing.Status = "Revoked";
            existing.Updatedat = DateTime.UtcNow;
            await _unitOfWork.UserCertificateRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Certificate revoked successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error revoking certificate: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserCertificateDto>>> GetUserCertificatesAsync(Guid userId)
    {
        try
        {
            var items = await _unitOfWork.UserCertificateRepository.GetByUserIdAsync(userId);
            return ServiceResult<List<UserCertificateDto>>.Success(items.Select(x => x.ToDto()).Where(x => x != null).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserCertificateDto>>.Failure($"Error retrieving user certificates: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserCertificateDto>>> GetCertificateHoldersAsync(Guid certificateId)
    {
        try
        {
            var items = await _unitOfWork.UserCertificateRepository.GetByCertificateIdAsync(certificateId);
            return ServiceResult<List<UserCertificateDto>>.Success(items.Select(x => x.ToDto()).Where(x => x != null).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserCertificateDto>>.Failure($"Error retrieving certificate holders: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserCertificateDto>>> GetExpiringCertificatesAsync(int daysAhead = 30)
    {
        try
        {
            var items = await _unitOfWork.UserCertificateRepository.GetExpiringCertificatesAsync(daysAhead);
            return ServiceResult<List<UserCertificateDto>>.Success(items.Select(x => x.ToDto()).Where(x => x != null).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserCertificateDto>>.Failure($"Error retrieving expiring certificates: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> HasCertificateAsync(Guid userId, Guid certificateId)
    {
        try
        {
            var hasCertificate = await _unitOfWork.UserCertificateRepository.HasCertificateAsync(userId, certificateId);
            return ServiceResult<bool>.Success(hasCertificate);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error checking certificate: {ex.Message}");
        }
    }
}