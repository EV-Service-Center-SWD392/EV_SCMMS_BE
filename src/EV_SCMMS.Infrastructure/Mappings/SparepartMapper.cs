using EV_SCMMS.Core.Application.DTOs.Sparepart;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for Sparepart entity mapping
/// </summary>
public static class SparepartMapperExtensions
{
    /// <summary>
    /// Map SparepartTuht entity to SparepartDto
    /// </summary>
    /// <param name="sparepart">SparepartTuht entity</param>
    /// <returns>SparepartDto</returns>
    public static SparepartDto ToDto(this SparepartTuht sparepart)
    {
        if (sparepart == null) return null;

        return new SparepartDto
        {
            SparepartId = sparepart.Sparepartid,
            VehicleModelId = sparepart.Vehiclemodelid,
            InventoryId = sparepart.Inventoryid ?? Guid.Empty,
            TypeId = sparepart.Typeid ?? Guid.Empty,
            Name = sparepart.Name,
            Description = null, // Not available in SparepartTuht model
            Manufacturer = sparepart.Manufacture,
            PartNumber = null, // Not available in SparepartTuht model
            UnitPrice = sparepart.Unitprice ?? 0,
            Status = sparepart.Status,
            IsActive = sparepart.Isactive ?? false,
            CreatedAt = sparepart.Createdat,
            UpdatedAt = sparepart.Updatedat,
            InventoryName = null, // Will need to be loaded separately
            TypeName = null
        };
    }

    /// <summary>
    /// Map list of SparepartTuht entities to list of SparepartDto
    /// </summary>
    /// <param name="spareparts">List of SparepartTuht entities</param>
    /// <returns>List of SparepartDto</returns>
    public static List<SparepartDto> ToDto(this IEnumerable<SparepartTuht> spareparts)
    {
        if (spareparts == null) return new List<SparepartDto>();
        
        return spareparts.Select(s => s.ToDto()).Where(dto => dto != null).ToList();
    }

    /// <summary>
    /// Map CreateSparepartDto to SparepartTuht entity
    /// </summary>
    /// <param name="createDto">CreateSparepartDto</param>
    /// <returns>SparepartTuht entity</returns>
    public static SparepartTuht ToEntity(this CreateSparepartDto createDto)
    {
        if (createDto == null) return null;

        return new SparepartTuht
        {
            Vehiclemodelid = createDto.VehicleModelId,
            Name = createDto.Name,
            Unitprice = createDto.UnitPrice,
            Manufacture = createDto.Manufacturer
        };
    }

    /// <summary>
    /// Update existing SparepartTuht entity with UpdateSparepartDto data
    /// </summary>
    /// <param name="entity">Existing SparepartTuht entity</param>
    /// <param name="updateDto">UpdateSparepartDto</param>
    public static void UpdateFromDto(this SparepartTuht entity, UpdateSparepartDto updateDto)
    {
        if (entity == null || updateDto == null) return;

        entity.Vehiclemodelid = updateDto.VehicleModelId;
        entity.Inventoryid = updateDto.InventoryId;
        entity.Typeid = updateDto.TypeId;
        entity.Name = updateDto.Name;
        entity.Unitprice = updateDto.UnitPrice;
        entity.Manufacture = updateDto.Manufacturer;
        entity.Status = updateDto.Status;
        entity.Updatedat = DateTime.UtcNow;
    }
}