using EV_SCMMS.Core.Application.DTOs.UserCertificate;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

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
            var hasExisting = await _unitOfWork.UserCertificateRepository.HasCertificateAsync(dto.UserId, dto.CertificateId);
            if (hasExisting)
            {
                return ServiceResult<UserCertificateDto>.Failure("User already has this certificate");
            }

            var entity = dto.ToEntity();
            entity.Status = "PENDING";
            
            await _unitOfWork.UserCertificateRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.UserCertificateRepository.GetByIdAsync(entity.Usercertificateid);
            var result = created?.ToDto();
            
            return ServiceResult<UserCertificateDto>.Success(result!, "Certificate assigned successfully");
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
            var entity = await _unitOfWork.UserCertificateRepository.GetByIdAsync(userCertificateId);
            if (entity == null)
            {
                return ServiceResult<bool>.Failure("Certificate assignment not found");
            }

            entity.Isactive = false;
            entity.Updatedat = DateTime.UtcNow;
            
            await _unitOfWork.UserCertificateRepository.UpdateAsync(entity);
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
            var certificates = await _unitOfWork.UserCertificateRepository.GetByUserIdAsync(userId);
            var result = certificates.Select(c => c.ToDto()).Where(c => c != null).ToList()!;
            
            return ServiceResult<List<UserCertificateDto>>.Success(result);
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
            var holders = await _unitOfWork.UserCertificateRepository.GetByCertificateIdAsync(certificateId);
            var result = holders.Select(h => h.ToDto()).Where(h => h != null).ToList()!;
            
            return ServiceResult<List<UserCertificateDto>>.Success(result);
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
            var expiring = await _unitOfWork.UserCertificateRepository.GetExpiringCertificatesAsync(daysAhead);
            var result = expiring.Select(e => e.ToDto()).Where(e => e != null).ToList()!;
            
            return ServiceResult<List<UserCertificateDto>>.Success(result);
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

    public async Task<IServiceResult<bool>> ApproveCertificateAsync(Guid userCertificateId)
    {
        try
        {
            var entity = await _unitOfWork.UserCertificateRepository.GetByIdAsync(userCertificateId);
            if (entity == null)
            {
                return ServiceResult<bool>.Failure("Certificate assignment not found");
            }

            entity.Status = "APPROVED";
            entity.Updatedat = DateTime.UtcNow;
            
            await _unitOfWork.UserCertificateRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Certificate approved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error approving certificate: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> RejectCertificateAsync(Guid userCertificateId)
    {
        try
        {
            var entity = await _unitOfWork.UserCertificateRepository.GetByIdAsync(userCertificateId);
            if (entity == null)
            {
                return ServiceResult<bool>.Failure("Certificate assignment not found");
            }

            if (entity.Status != "PENDING")
            {
                return ServiceResult<bool>.Failure("Only pending certificates can be rejected");
            }

            entity.Status = "REJECTED";
            entity.Isactive = false;
            entity.Updatedat = DateTime.UtcNow;
            
            await _unitOfWork.UserCertificateRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Certificate rejected successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error rejecting certificate: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserCertificateDto>>> GetPendingCertificatesAsync()
    {
        try
        {
            var pendingCertificates = await _unitOfWork.UserCertificateRepository.GetPendingCertificatesAsync();
            var result = pendingCertificates.Select(c => c.ToDto()).Where(c => c != null).ToList()!;
            
            return ServiceResult<List<UserCertificateDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserCertificateDto>>.Failure($"Error retrieving pending certificates: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<UserCertificateDto>>> GetAllUserCertificatesAsync()
    {
        try
        {
            var allUserCertificates = await _unitOfWork.UserCertificateRepository.GetAllWithDetailsAsync();
            var result = allUserCertificates.Select(c => c.ToDto()).Where(c => c != null).ToList()!;
            
            return ServiceResult<List<UserCertificateDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserCertificateDto>>.Failure($"Error retrieving all user certificates: {ex.Message}");
        }
    }

    public async Task<IServiceResult<object>> GetCertificateExpiryStatusAsync(Guid certificateId)
    {
        try
        {
            var holders = await _unitOfWork.UserCertificateRepository.GetByCertificateIdAsync(certificateId);
            var result = holders.ToCertificateExpiryStatusDto(certificateId);
            
            return ServiceResult<object>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<object>.Failure($"Error retrieving certificate expiry status: {ex.Message}");
        }
    }

    public async Task<IServiceResult<object>> ValidateUserCertificateAsync(Guid userId, Guid certificateId)
    {
        try
        {
            var userCertificates = await _unitOfWork.UserCertificateRepository.GetByUserIdAsync(userId);
            var certificate = userCertificates.FirstOrDefault(c => c.Certificateid == certificateId);
            var result = certificate.ToValidationDto();
            
            return ServiceResult<object>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<object>.Failure($"Error validating certificate: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> SetCertificateStatusRevokedAsync(Guid userCertificateId)
    {
        try
        {
            var entity = await _unitOfWork.UserCertificateRepository.GetByIdAsync(userCertificateId);
            if (entity == null)
            {
                return ServiceResult<bool>.Failure("Certificate assignment not found");
            }

            entity.Status = "REVOKED";
            entity.Isactive = false;
            entity.Updatedat = DateTime.UtcNow;
            
            await _unitOfWork.UserCertificateRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Certificate status set to REVOKED successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error setting certificate status to REVOKED: {ex.Message}");
        }
    }
}