using EV_SCMMS.Core.Application.DTOs.UserRole;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

public static class UserRoleMapperExtensions
{
    public static UserRoleDto ToDto(this Userrole entity)
    {
        if (entity == null) return null;

        return new UserRoleDto
        {
            RoleId = entity.Roleid,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status,
            IsActive = entity.Isactive ?? false,
            CreatedAt = entity.Createdat,
            UpdatedAt = entity.Updatedat
        };
    }

    public static Userrole ToEntity(this CreateUserRoleDto createDto)
    {
        if (createDto == null) return null;

        return new Userrole
        {
            Roleid = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            Status = createDto.Status,
            Isactive = true,
            Createdat = DateTime.UtcNow
        };
    }

    public static void UpdateFromDto(this Userrole entity, UpdateRoleDto updateDto)
    {
        if (entity == null || updateDto == null) return;

        if (!string.IsNullOrEmpty(updateDto.Name))
            entity.Name = updateDto.Name;
        
        entity.Description = updateDto.Description;
        entity.Status = updateDto.Status;
        entity.Isactive = updateDto.IsActive;
        entity.Updatedat = DateTime.UtcNow;
    }
}