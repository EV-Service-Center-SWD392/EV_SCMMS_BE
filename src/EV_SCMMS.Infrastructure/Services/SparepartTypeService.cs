using EV_SCMMS.Core.Application.DTOs.SparepartType;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// SparepartType service implementation for business logic operations
/// </summary>
public class SparepartTypeService : ISparepartTypeService
{
    private readonly IUnitOfWork _unitOfWork;

    public SparepartTypeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<List<SparepartTypeDto>>> GetAllAsync()
    {
        try
        {
            var allResults = await _unitOfWork.SparepartTypeRepository.GetAllAsync();

            if (allResults == null || !allResults.Any())
            {
                return ServiceResult<List<SparepartTypeDto>>.Failure("No sparepart types found");
            }
            
            var sparepartTypeDtos = allResults.ToDto();

            return ServiceResult<List<SparepartTypeDto>>.Success(sparepartTypeDtos, "Sparepart types retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartTypeDto>>.Failure($"Error retrieving sparepart types: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartTypeDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var sparepartType = await _unitOfWork.SparepartTypeRepository.GetByIdAsync(id);
            if (sparepartType == null)
            {
                return ServiceResult<SparepartTypeDto>.Failure("Sparepart type not found");
            }

            var sparepartTypeDto = sparepartType.ToDto();
            return ServiceResult<SparepartTypeDto>.Success(sparepartTypeDto, "Sparepart type retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartTypeDto>.Failure($"Error retrieving sparepart type: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartTypeDto>> CreateAsync(CreateSparepartTypeDto createDto)
    {
        try
        {
            // Check if sparepart type name already exists
            if (await _unitOfWork.SparepartTypeRepository.IsNameExistsAsync(createDto.Name))
            {
                return ServiceResult<SparepartTypeDto>.Failure("Sparepart type name already exists");
            }

            var sparepartType = createDto.ToEntity();
            sparepartType.Typeid = Guid.NewGuid();
            sparepartType.Createdat = DateTime.UtcNow;
            sparepartType.Isactive = true;

            var createdSparepartType = await _unitOfWork.SparepartTypeRepository.AddAsync(sparepartType);
            await _unitOfWork.SaveChangesAsync();

            var sparepartTypeDto = createdSparepartType.ToDto();
            return ServiceResult<SparepartTypeDto>.Success(sparepartTypeDto, "Sparepart type created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartTypeDto>.Failure($"Error creating sparepart type: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartTypeDto>> UpdateAsync(Guid id, UpdateSparepartTypeDto updateDto)
    {
        try
        {
            var sparepartType = await _unitOfWork.SparepartTypeRepository.GetByIdAsync(id);
            if (sparepartType == null)
            {
                return ServiceResult<SparepartTypeDto>.Failure("Sparepart type not found");
            }

            // Check if sparepart type name already exists (excluding current type)
            if (await _unitOfWork.SparepartTypeRepository.IsNameExistsAsync(updateDto.Name, id))
            {
                return ServiceResult<SparepartTypeDto>.Failure("Sparepart type name already exists");
            }

            sparepartType.UpdateFromDto(updateDto);
            sparepartType.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SparepartTypeRepository.UpdateAsync(sparepartType);
            await _unitOfWork.SaveChangesAsync();

            var sparepartTypeDto = sparepartType.ToDto();
            return ServiceResult<SparepartTypeDto>.Success(sparepartTypeDto, "Sparepart type updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartTypeDto>.Failure($"Error updating sparepart type: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var sparepartType = await _unitOfWork.SparepartTypeRepository.GetByIdAsync(id);
            if (sparepartType == null)
            {
                return ServiceResult<bool>.Failure("Sparepart type not found");
            }

            // Check if there are any spareparts using this type
            var spareparts = await _unitOfWork.SparepartRepository.GetByTypeIdAsync(id);
            if (spareparts.Any())
            {
                return ServiceResult<bool>.Failure("Cannot delete sparepart type because it is being used by spareparts");
            }

            await _unitOfWork.SparepartTypeRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Sparepart type deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting sparepart type: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartTypeDto>>> GetActiveTypesAsync()
    {
        try
        {
            var sparepartTypes = await _unitOfWork.SparepartTypeRepository.GetActiveSparepartTypesAsync();
            var sparepartTypeDtos = sparepartTypes.ToDto();

            return ServiceResult<List<SparepartTypeDto>>.Success(sparepartTypeDtos, "Active sparepart types retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartTypeDto>>.Failure($"Error retrieving active sparepart types: {ex.Message}");
        }
    }

    public Task<IServiceResult<List<SparepartTypeDto>>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10)
    {
        throw new NotImplementedException();
    }
}