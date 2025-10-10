using EV_SCMMS.Core.Application.DTOs.Center;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Center service implementation for business logic operations
/// </summary>
public class CenterService : ICenterService
{
    private readonly IUnitOfWork _unitOfWork;

    public CenterService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<CenterDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var center = await _unitOfWork.CenterRepository.GetByIdAsync(id);
            if (center == null)
            {
                return ServiceResult<CenterDto>.Failure("Center not found");
            }

            var centerDto = center.ToDto();
            return ServiceResult<CenterDto>.Success(centerDto, "Center retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<CenterDto>.Failure($"Error retrieving center: {ex.Message}");
        }
    }

    public async Task<IServiceResult<CenterDto>> CreateAsync(CreateCenterDto createDto)
    {
        try
        {
            // // Check if center name already exists
            // if (await _unitOfWork.CenterRepository.IsNameExistsAsync(createDto.Name))
            // {
            //     return ServiceResult<CenterDto>.Failure("Center name already exists");
            // }

            var center = createDto.ToEntity();

            var createdCenter = await _unitOfWork.CenterRepository.AddAsync(center);
            await _unitOfWork.SaveChangesAsync();

            var centerDto = createdCenter.ToDto();
            return ServiceResult<CenterDto>.Success(centerDto, "Center created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<CenterDto>.Failure($"Error creating center: {ex.Message}");
        }
    }

    public async Task<IServiceResult<CenterDto>> UpdateAsync(Guid id, UpdateCenterDto updateDto)
    {
        try
        {
            var center = await _unitOfWork.CenterRepository.GetByIdAsync(id);
            if (center == null)
            {
                return ServiceResult<CenterDto>.Failure("Center not found");
            }

            // // Check if center name already exists (excluding current center)
            // if (await _unitOfWork.CenterRepository.IsNameExistsAsync(updateDto.Name, id))
            // {
            //     return ServiceResult<CenterDto>.Failure("Center name already exists");
            // }

            center.UpdateFromDto(updateDto);

            await _unitOfWork.CenterRepository.UpdateAsync(center);

            var centerDto = center.ToDto();
            return ServiceResult<CenterDto>.Success(centerDto, "Center updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<CenterDto>.Failure($"Error updating center: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var center = await _unitOfWork.CenterRepository.GetByIdAsync(id);
            if (center == null)
            {
                return ServiceResult<bool>.Failure("Center not found");
            }

            await _unitOfWork.CenterRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Center deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting center: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<CenterDto>>> GetActiveCentersAsync()
    {
        try
        {
            var centers = await _unitOfWork.CenterRepository.GetActiveCentersAsync();
            var centerDtos = centers.ToDto();

            return ServiceResult<List<CenterDto>>.Success(centerDtos, "Active centers retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<CenterDto>>.Failure($"Error retrieving active centers: {ex.Message}");
        }
    }

    public Task<IServiceResult<List<CenterDto>>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    public Task<IServiceResult<List<CenterDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        throw new NotImplementedException();
    }
}