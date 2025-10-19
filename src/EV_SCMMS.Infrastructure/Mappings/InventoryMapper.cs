using EV_SCMMS.Core.Application.DTOs.Inventory;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for Inventory entity mapping
/// </summary>
public static class InventoryMapperExtensions
{
    /// <summary>
    /// Map InventoryTuht entity to InventoryDto
    /// </summary>
    /// <param name="inventory">InventoryTuht entity</param>
    /// <returns>InventoryDto</returns>
    public static InventoryDto ToDto(this InventoryTuht inventory)
    {
        if (inventory == null) return null;

        return new InventoryDto
        {
            InventoryId = inventory.Inventoryid,
            CenterId = inventory.Centerid,
            Name = null, // Not available in InventoryTuht model
            Description = null, // Not available in InventoryTuht model
            Location = null, // Not available in InventoryTuht model
            MinimumStockLevel = inventory.Minimumstocklevel,
            Status = inventory.Status,
            IsActive = inventory.Isactive ?? false,
            CreatedAt = inventory.Createdat,
            UpdatedAt = inventory.Updatedat,
            CenterName = null // Remove navigation property access to prevent lazy loading
        };
    }

    /// <summary>
    /// Map list of InventoryTuht entities to list of InventoryDto
    /// </summary>
    /// <param name="inventories">List of InventoryTuht entities</param>
    /// <returns>List of InventoryDto</returns>
    public static List<InventoryDto> ToDto(this IEnumerable<InventoryTuht> inventories)
    {
        if (inventories == null) return new List<InventoryDto>();
        
        return inventories.Select(i => i.ToDto()).Where(dto => dto != null).ToList();
    }

    /// <summary>
    /// Map CreateInventoryDto to InventoryTuht entity
    /// </summary>
    /// <param name="createDto">CreateInventoryDto</param>
    /// <returns>InventoryTuht entity</returns>
    public static InventoryTuht ToEntity(this CreateInventoryDto createDto)
    {
        if (createDto == null) return null;

        return new InventoryTuht
        {
            Inventoryid = Guid.NewGuid(),
            Centerid = createDto.CenterId,
            Quantity = 0, // Default to 0
            Minimumstocklevel = createDto.MinimumStockLevel,
            Status = createDto.Status ?? "Active",
            Isactive = true,
            Createdat = DateTime.UtcNow,
            Updatedat = null
        };
    }

    /// <summary>
    /// Update existing InventoryTuht entity with UpdateInventoryDto data
    /// </summary>
    /// <param name="entity">Existing InventoryTuht entity</param>
    /// <param name="updateDto">UpdateInventoryDto</param>
    public static void UpdateFromDto(this InventoryTuht entity, UpdateInventoryDto updateDto)
    {
        if (entity == null || updateDto == null) return;

        entity.Centerid = updateDto.CenterId;
        entity.Minimumstocklevel = updateDto.MinimumStockLevel;
        entity.Status = updateDto.Status;
        entity.Updatedat = DateTime.UtcNow;
    }
}