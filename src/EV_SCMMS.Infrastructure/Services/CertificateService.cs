using EV_SCMMS.Core.Application.DTOs.Certificate;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Service implementation for certificate management
/// </summary>
public class CertificateService : ICertificateService
{
    private readonly IUnitOfWork _unitOfWork;

    public CertificateService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<CertificateDto>> CreateAsync(CreateCertificateDto dto)
    {
        try
        {
            var existing = await _unitOfWork.CertificateRepository.GetByNameAsync(dto.Name);
            if (existing != null)
            {
                return ServiceResult<CertificateDto>.Failure("Certificate name already exists");
            }

            var entity = dto.ToEntity();
            await _unitOfWork.CertificateRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<CertificateDto>.Success(entity.ToDto(), "Certificate created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<CertificateDto>.Failure($"Error creating certificate: {ex.Message}");
        }
    }

    public async Task<IServiceResult<CertificateDto>> UpdateAsync(Guid id, UpdateCertificateDto dto)
    {
        try
        {
            var existing = await _unitOfWork.CertificateRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ServiceResult<CertificateDto>.Failure("Certificate not found");
            }

            existing.UpdateFromDto(dto);
            await _unitOfWork.CertificateRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<CertificateDto>.Success(existing.ToDto(), "Certificate updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<CertificateDto>.Failure($"Error updating certificate: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var existing = await _unitOfWork.CertificateRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ServiceResult<bool>.Failure("Certificate not found");
            }

            existing.Isactive = false;
            existing.Updatedat = DateTime.UtcNow;
            await _unitOfWork.CertificateRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Certificate deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting certificate: {ex.Message}");
        }
    }

    public async Task<IServiceResult<CertificateDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _unitOfWork.CertificateRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<CertificateDto>.Failure("Certificate not found");
            }

            return ServiceResult<CertificateDto>.Success(entity.ToDto());
        }
        catch (Exception ex)
        {
            return ServiceResult<CertificateDto>.Failure($"Error retrieving certificate: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<CertificateDto>>> GetAllAsync()
    {
        try
        {
            var items = await _unitOfWork.CertificateRepository.GetAllAsync();
            return ServiceResult<List<CertificateDto>>.Success(items.Select(x => x.ToDto()).Where(x => x != null).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<CertificateDto>>.Failure($"Error retrieving certificates: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<CertificateDto>>> GetActiveCertificatesAsync()
    {
        try
        {
            var items = await _unitOfWork.CertificateRepository.GetActiveCertificatesAsync();
            return ServiceResult<List<CertificateDto>>.Success(items.Select(x => x.ToDto()).Where(x => x != null).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<List<CertificateDto>>.Failure($"Error retrieving active certificates: {ex.Message}");
        }
    }
}