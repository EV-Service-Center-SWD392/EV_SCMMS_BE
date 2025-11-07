using EV_SCMMS.Core.Application.DTOs.UserCertificate;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

public static class UserCertificateMapperExtensions
{
    public static UserCertificateDto? ToDto(this Usercertificatetuantm? entity)
    {
        if (entity == null) return null;

        var expiryDate = entity.Createdat?.AddYears(1);
        var daysUntilExpiry = expiryDate.HasValue ? (int)(expiryDate.Value - DateTime.UtcNow).TotalDays : 0;

        return new UserCertificateDto
        {
            UserCertificateId = entity.Usercertificateid,
            UserId = entity.Userid,
            CertificateId = entity.Certificateid,
            Status = entity.Status,
            IsActive = entity.Isactive ?? false,
            CreatedAt = entity.Createdat,
            UpdatedAt = entity.Updatedat,
            UserName = $"{entity.User?.Firstname} {entity.User?.Lastname}".Trim(),
            CertificateName = entity.Certificate?.Name,
            ExpiryDate = expiryDate,
            IsExpired = expiryDate.HasValue && expiryDate.Value < DateTime.UtcNow,
            DaysUntilExpiry = Math.Max(0, daysUntilExpiry)
        };
    }

    public static Usercertificatetuantm? ToEntity(this AssignCertificateDto? assignDto)
    {
        if (assignDto == null) return null;

        return new Usercertificatetuantm
        {
            Usercertificateid = Guid.NewGuid(),
            Userid = assignDto.UserId,
            Certificateid = assignDto.CertificateId,
            Status = assignDto.Status,
            Isactive = true,
            Createdat = DateTime.UtcNow
        };
    }
}