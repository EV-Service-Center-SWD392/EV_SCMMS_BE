using EV_SCMMS.Core.Application.DTOs.SparepartType;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for SparepartType entity mapping
/// </summary>
public static class SparepartTypeMapperExtensions
{
    /// <summary>
    /// Map SpareparttypeTuht entity to SparepartTypeDto
    /// </summary>
    /// <param name="sparepartType">SpareparttypeTuht entity</param>
    /// <returns>SparepartTypeDto</returns>
    public static SparepartTypeDto ToDto(this SpareparttypeTuht sparepartType)
    {
        if (sparepartType == null) return null;

        return new SparepartTypeDto
        {
            TypeId = sparepartType.Typeid,
            Name = sparepartType.Name,
            Description = sparepartType.Description,
            Status = sparepartType.Status,
            IsActive = sparepartType.Isactive ?? false,
            CreatedAt = sparepartType.Createdat,
            UpdatedAt = sparepartType.Updatedat
        };
    }

    /// <summary>
    /// Map list of SpareparttypeTuht entities to list of SparepartTypeDto
    /// </summary>
    /// <param name="sparepartTypes">List of SpareparttypeTuht entities</param>
    /// <returns>List of SparepartTypeDto</returns>
    public static List<SparepartTypeDto> ToDto(this IEnumerable<SpareparttypeTuht> sparepartTypes)
    {
        if (sparepartTypes == null) return new List<SparepartTypeDto>();
        
        return sparepartTypes.Select(st => st.ToDto()).Where(dto => dto != null).ToList();
    }

    /// <summary>
    /// Map CreateSparepartTypeDto to SpareparttypeTuht entity
    /// </summary>
    /// <param name="createDto">CreateSparepartTypeDto</param>
    /// <returns>SpareparttypeTuht entity</returns>
    public static SpareparttypeTuht ToEntity(this CreateSparepartTypeDto createDto)
    {
        if (createDto == null) return null;

        return new SpareparttypeTuht
        {
            Typeid = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            Status = createDto.Status ?? "Active",
            Isactive = true,
            Createdat = DateTime.UtcNow,
            Updatedat = null
        };
    }

    /// <summary>
    /// Update existing SpareparttypeTuht entity with UpdateSparepartTypeDto data
    /// </summary>
    /// <param name="entity">Existing SpareparttypeTuht entity</param>
    /// <param name="updateDto">UpdateSparepartTypeDto</param>
    public static void UpdateFromDto(this SpareparttypeTuht entity, UpdateSparepartTypeDto updateDto)
    {
        if (entity == null || updateDto == null) return;

        entity.Name = updateDto.Name;
        entity.Description = updateDto.Description;
        entity.Status = updateDto.Status;
        entity.Updatedat = DateTime.UtcNow;
    }
}