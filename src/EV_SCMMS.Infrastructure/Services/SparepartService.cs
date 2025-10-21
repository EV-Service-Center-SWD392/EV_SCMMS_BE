using EV_SCMMS.Core.Application.DTOs.Sparepart;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Sparepart service implementation for business logic operations
/// </summary>
public class SparepartService : ISparepartService
{
    private readonly IUnitOfWork _unitOfWork;

    public SparepartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<List<SparepartDto>>> GetAllAsync()
    {
        try
        {
            var allResults = await _unitOfWork.SparepartRepository.GetAllAsync();

            if (allResults == null || !allResults.Any())
            {
                return ServiceResult<List<SparepartDto>>.Failure("No spareparts found");
            }
            
            var sparepartDtos = allResults.ToDto();

            return ServiceResult<List<SparepartDto>>.Success(sparepartDtos, "Spareparts retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartDto>>.Failure($"Error retrieving spareparts: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var sparepart = await _unitOfWork.SparepartRepository.GetByIdAsync(id);
            if (sparepart == null)
            {
                return ServiceResult<SparepartDto>.Failure("Sparepart not found");
            }

            var sparepartDto = sparepart.ToDto();
            return ServiceResult<SparepartDto>.Success(sparepartDto, "Sparepart retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartDto>.Failure($"Error retrieving sparepart: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartDto>> CreateAsync(CreateSparepartDto createDto)
    {
        try
        {
            // Check if inventory exists
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(createDto.InventoryId);
            if (inventory == null)
            {
                return ServiceResult<SparepartDto>.Failure("Inventory not found");
            }

            // Check if sparepart type exists
            var sparepartType = await _unitOfWork.SparepartTypeRepository.GetByIdAsync(createDto.TypeId);
            if (sparepartType == null)
            {
                return ServiceResult<SparepartDto>.Failure("Sparepart type not found");
            }

            // Check if sparepart name already exists
            if (await _unitOfWork.SparepartRepository.IsNameExistsAsync(createDto.Name))
            {
                return ServiceResult<SparepartDto>.Failure("Sparepart name already exists");
            }

            var sparepart = createDto.ToEntity();
            sparepart.Sparepartid = Guid.NewGuid();
            sparepart.Createdat = DateTime.UtcNow;
            sparepart.Isactive = true;

            var createdSparepart = await _unitOfWork.SparepartRepository.AddAsync(sparepart);

            var sparepartDto = createdSparepart.ToDto();
            return ServiceResult<SparepartDto>.Success(sparepartDto, "Sparepart created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartDto>.Failure($"Error creating sparepart: {ex.Message}");
        }
    }

    public async Task<IServiceResult<SparepartDto>> UpdateAsync(Guid id, UpdateSparepartDto updateDto)
    {
        try
        {
            var sparepart = await _unitOfWork.SparepartRepository.GetByIdAsync(id);
            if (sparepart == null)
            {
                return ServiceResult<SparepartDto>.Failure("Sparepart not found");
            }

            // Check if inventory exists
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(updateDto.InventoryId);
            if (inventory == null)
            {
                return ServiceResult<SparepartDto>.Failure("Inventory not found");
            }

            // Check if sparepart type exists
            var sparepartType = await _unitOfWork.SparepartTypeRepository.GetByIdAsync(updateDto.TypeId);
            if (sparepartType == null)
            {
                return ServiceResult<SparepartDto>.Failure("Sparepart type not found");
            }

            // Check if sparepart name already exists (excluding current sparepart)
            if (await _unitOfWork.SparepartRepository.IsNameExistsAsync(updateDto.Name, id))
            {
                return ServiceResult<SparepartDto>.Failure("Sparepart name already exists");
            }

            sparepart.UpdateFromDto(updateDto);
            sparepart.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SparepartRepository.UpdateAsync(sparepart);

            var sparepartDto = sparepart.ToDto();
            return ServiceResult<SparepartDto>.Success(sparepartDto, "Sparepart updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<SparepartDto>.Failure($"Error updating sparepart: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var sparepart = await _unitOfWork.SparepartRepository.GetByIdAsync(id);
            if (sparepart == null)
            {
                return ServiceResult<bool>.Failure("Sparepart not found");
            }

            await _unitOfWork.SparepartRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Sparepart deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting sparepart: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartDto>>> GetByTypeIdAsync(Guid typeId)
    {
        try
        {
            var spareparts = await _unitOfWork.SparepartRepository.GetByTypeIdAsync(typeId);
            var sparepartDtos = spareparts.ToDto();

            return ServiceResult<List<SparepartDto>>.Success(sparepartDtos, "Spareparts retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartDto>>.Failure($"Error retrieving spareparts by type: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartDto>>> GetByVehicleModelIdAsync(Guid vehicleModelId)
    {
        try
        {
            // Note: The repository method expects int, but interface expects Guid
            // This might need adjustment based on your actual vehicle model ID type
            var vehicleModelIdInt = (int)vehicleModelId.GetHashCode(); // Temporary conversion
            var spareparts = await _unitOfWork.SparepartRepository.GetByVehicleModelIdAsync(vehicleModelIdInt);
            var sparepartDtos = spareparts.ToDto();

            return ServiceResult<List<SparepartDto>>.Success(sparepartDtos, "Spareparts retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartDto>>.Failure($"Error retrieving spareparts by vehicle model: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<SparepartDto>>> GetByManufacturerAsync(string manufacturer)
    {
        try
        {
            var spareparts = await _unitOfWork.SparepartRepository.GetByManufacturerAsync(manufacturer);
            var sparepartDtos = spareparts.ToDto();

            return ServiceResult<List<SparepartDto>>.Success(sparepartDtos, "Spareparts retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SparepartDto>>.Failure($"Error retrieving spareparts by manufacturer: {ex.Message}");
        }
    }

    public Task<IServiceResult<List<SparepartDto>>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    public Task<IServiceResult<List<SparepartDto>>> SearchAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }
}