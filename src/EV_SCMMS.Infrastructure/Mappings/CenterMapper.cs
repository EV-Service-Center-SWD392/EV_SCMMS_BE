using EV_SCMMS.Core.Application.DTOs.Center;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for Center entity mapping
/// </summary>
public static class CenterMapperExtensions
{
    /// <summary>
    /// Map Center entity to CenterDto
    /// </summary>
    /// <param name="center">Center entity</param>
    /// <returns>CenterDto</returns>
    public static CenterDto ToDto(this Centertuantm center)
    {
        if (center == null) return null;

        return new CenterDto
        {
            CenterId = center.Centerid,
            Name = center.Name,
            Address = center.Address,
            Phone = null, // Not available in Center model
            Email = null, // Not available in Center model
            Status = center.Status,
            IsActive = center.Isactive ?? false,
            CreatedAt = center.Createdat,
            UpdatedAt = center.Updatedat
        };
    }

    /// <summary>
    /// Map list of Center entities to list of CenterDto
    /// </summary>
    /// <param name="centers">List of Center entities</param>
    /// <returns>List of CenterDto</returns>
    public static List<CenterDto> ToDto(this IEnumerable<Centertuantm> centers)
    {
        if (centers == null) return new List<CenterDto>();
        
        return centers.Select(c => c.ToDto()).Where(dto => dto != null).ToList();
    }

    /// <summary>
    /// Map CreateCenterDto to Center entity
    /// </summary>
    /// <param name="createDto">CreateCenterDto</param>
    /// <returns>Center entity</returns>
    public static Centertuantm ToEntity(this CreateCenterDto createDto)
    {
        if (createDto == null) return null;

        return new Centertuantm
        {
            Centerid = Guid.NewGuid(),
            Name = createDto.Name,
            Address = createDto.Address,
            Status = createDto.Status ?? "Active",
            Isactive = true,
            Createdat = DateTime.UtcNow,
            Updatedat = null
        };
    }

    /// <summary>
    /// Update existing Center entity with UpdateCenterDto data
    /// </summary>
    /// <param name="entity">Existing Center entity</param>
    /// <param name="updateDto">UpdateCenterDto</param>
    public static void UpdateFromDto(this Centertuantm entity, UpdateCenterDto updateDto)
    {
        if (entity == null || updateDto == null) return;

        entity.Name = updateDto.Name;
        entity.Address = updateDto.Address;
        entity.Status = updateDto.Status;
        entity.Updatedat = DateTime.UtcNow;
    }
}