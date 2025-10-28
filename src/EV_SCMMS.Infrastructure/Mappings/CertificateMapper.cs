using EV_SCMMS.Core.Application.DTOs.Certificate;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for Certificate entity mapping
/// </summary>
public static class CertificateMapperExtensions
{
    public static CertificateDto ToDto(this Certificatetuantm entity)
    {
        if (entity == null) return null;

        return new CertificateDto
        {
            CertificateId = entity.Certificateid,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status,
            IsActive = entity.Isactive ?? false,
            CreatedAt = entity.Createdat,
            UpdatedAt = entity.Updatedat
        };
    }

    public static Certificatetuantm ToEntity(this CreateCertificateDto createDto)
    {
        if (createDto == null) return null;

        return new Certificatetuantm
        {
            Certificateid = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            Status = createDto.Status,
            Isactive = true,
            Createdat = DateTime.UtcNow
        };
    }

    public static void UpdateFromDto(this Certificatetuantm entity, UpdateCertificateDto updateDto)
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