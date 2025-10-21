using EV_SCMMS.Core.Application.DTOs.Inventory;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Inventory service interface for business logic operations
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Get all inventories
    /// </summary>
    /// <returns>List of all inventories</returns>
    Task<IServiceResult<IEnumerable<InventoryDto>>> GetAllAsync();

    /// <summary>
    /// Get inventory by ID
    /// </summary>
    /// <param name="id">Inventory ID</param>
    /// <returns>Inventory DTO</returns>
    Task<IServiceResult<InventoryDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Create new inventory
    /// </summary>
    /// <param name="createDto">Inventory creation DTO</param>
    /// <returns>Created inventory DTO</returns>
    Task<IServiceResult<InventoryDto>> CreateAsync(CreateInventoryDto createDto);

    /// <summary>
    /// Update existing inventory
    /// </summary>
    /// <param name="id">Inventory ID</param>
    /// <param name="updateDto">Inventory update DTO</param>
    /// <returns>Updated inventory DTO</returns>
    Task<IServiceResult<InventoryDto>> UpdateAsync(Guid id, UpdateInventoryDto updateDto);

    /// <summary>
    /// Soft delete inventory
    /// </summary>
    /// <param name="id">Inventory ID</param>
    /// <returns>Result of deletion</returns>
    Task<IServiceResult<bool>> DeleteAsync(Guid id);

    /// <summary>
    /// Get inventories by center ID
    /// </summary>
    /// <param name="centerId">Center ID</param>
    /// <returns>List of inventories for the center</returns>
    Task<IServiceResult<List<InventoryDto>>> GetByCenterIdAsync(Guid centerId);

    /// <summary>
    /// Get low stock inventories
    /// </summary>
    /// <returns>List of inventories with low stock</returns>
    Task<IServiceResult<List<InventoryDto>>> GetLowStockInventoriesAsync();

    /// <summary>
    /// Update inventory quantity
    /// </summary>
    /// <param name="id">Inventory ID</param>
    /// <param name="quantity">New quantity</param>
    /// <returns>Updated inventory DTO</returns>
    Task<IServiceResult<InventoryDto>> UpdateQuantityAsync(Guid id, int quantity);
}