using EV_SCMMS.Core.Application.DTOs.Inventory;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Mappings;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Inventory service implementation for business logic operations
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IServiceResult<IEnumerable<InventoryDto>>> GetAllAsync()
    {
        try
        {
            var inventories = await _unitOfWork.InventoryRepository.GetAllWithProperties();

            if (inventories == null || !inventories.Any())
            {
                return ServiceResult<IEnumerable<InventoryDto>>.Failure("No inventories found");
            }
            
            var inventoryDtos = inventories.ToDto();

            return ServiceResult<IEnumerable<InventoryDto>>.Success(inventoryDtos, "Inventories retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<InventoryDto>>.Failure($"Error retrieving inventories: {ex.Message}");
        }
    }

    public async Task<IServiceResult<InventoryDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(id);
            if (inventory == null)
            {
                return ServiceResult<InventoryDto>.Failure("Inventory not found");
            }

            var inventoryDto = inventory.ToDto();
            return ServiceResult<InventoryDto>.Success(inventoryDto, "Inventory retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<InventoryDto>.Failure($"Error retrieving inventory: {ex.Message}");
        }
    }

    public async Task<IServiceResult<InventoryDto>> CreateAsync(CreateInventoryDto createDto)
    {
        try
        {
            // Check if center exists
            var center = await _unitOfWork.CenterRepository.GetByIdAsync(createDto.CenterId);
            if (center == null)
            {
                return ServiceResult<InventoryDto>.Failure("Center not found");
            }

            var inventory = createDto.ToEntity();

            var createdInventory = await _unitOfWork.InventoryRepository.AddAsync(inventory);

            var inventoryDto = createdInventory.ToDto();
            return ServiceResult<InventoryDto>.Success(inventoryDto, "Inventory created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<InventoryDto>.Failure($"Error creating inventory: {ex.Message}");
        }
    }

    public async Task<IServiceResult<InventoryDto>> UpdateAsync(Guid id, UpdateInventoryDto updateDto)
    {
        try
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(id);
            if (inventory == null)
            {
                return ServiceResult<InventoryDto>.Failure("Inventory not found");
            }

            inventory.UpdateFromDto(updateDto);

            await _unitOfWork.InventoryRepository.UpdateAsync(inventory);

            var inventoryDto = inventory.ToDto();
            return ServiceResult<InventoryDto>.Success(inventoryDto, "Inventory updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<InventoryDto>.Failure($"Error updating inventory: {ex.Message}");
        }
    }

    public async Task<IServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(id);
            if (inventory == null)
            {
                return ServiceResult<bool>.Failure("Inventory not found");
            }

            await _unitOfWork.InventoryRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Inventory deleted successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Error deleting inventory: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<InventoryDto>>> GetByCenterIdAsync(Guid centerId)
    {
        try
        {
            var inventories = await _unitOfWork.InventoryRepository.GetByCenterIdAsync(centerId);
            var inventoryDtos = inventories.ToDto();

            return ServiceResult<List<InventoryDto>>.Success(inventoryDtos, "Inventories retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<InventoryDto>>.Failure($"Error retrieving inventories by center: {ex.Message}");
        }
    }

    public async Task<IServiceResult<List<InventoryDto>>> GetLowStockInventoriesAsync()
    {
        try
        {
            var inventories = await _unitOfWork.InventoryRepository.GetLowStockInventoriesAsync();
            var inventoryDtos = inventories.ToDto();

            return ServiceResult<List<InventoryDto>>.Success(inventoryDtos, "Low stock inventories retrieved successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<InventoryDto>>.Failure($"Error retrieving low stock inventories: {ex.Message}");
        }
    }

    public async Task<IServiceResult<InventoryDto>> UpdateQuantityAsync(Guid id, int quantity)
    {
        try
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(id);
            if (inventory == null)
            {
                return ServiceResult<InventoryDto>.Failure("Inventory not found");
            }

            inventory.Updatedat = DateTime.UtcNow;

            await _unitOfWork.InventoryRepository.UpdateAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            var inventoryDto = inventory.ToDto();
            return ServiceResult<InventoryDto>.Success(inventoryDto, "Inventory quantity updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<InventoryDto>.Failure($"Error updating inventory quantity: {ex.Message}");
        }
    }
}