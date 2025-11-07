using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

public static class CertificateExpiryStatusMapper
{
    public static object ToCertificateExpiryStatusDto(this List<Usercertificatetuantm> holders, Guid certificateId)
    {
        var activeHolders = holders.Where(h => h.Status == "Approved" && h.Isactive == true).ToList();
        
        return new
        {
            CertificateId = certificateId,
            TotalActiveHolders = activeHolders.Count,
            Holders = activeHolders.Select(h => h.ToHolderDto()).ToList()
        };
    }

    public static object ToHolderDto(this Usercertificatetuantm entity)
    {
        return new
        {
            UserId = entity.Userid,
            UserName = $"{entity.User?.Firstname} {entity.User?.Lastname}".Trim(),
            AssignedDate = entity.Createdat,
            ExpiryDate = entity.Createdat?.AddYears(1),
            IsExpired = entity.Createdat?.AddYears(1) < DateTime.UtcNow,
            DaysUntilExpiry = entity.Createdat.HasValue ? 
                Math.Max(0, (int)(entity.Createdat.Value.AddYears(1) - DateTime.UtcNow).TotalDays) : 0
        };
    }

    public static object ToValidationDto(this Usercertificatetuantm? certificate)
    {
        if (certificate == null)
        {
            return new { isValid = false, isActive = false, message = "Certificate not assigned to user" };
        }

        var isExpired = certificate.Createdat?.AddYears(1) < DateTime.UtcNow;
        var isValid = certificate.Status == "Approved" && certificate.Isactive == true && !isExpired;
        
        return new
        {
            isValid = isValid,
            isActive = certificate.Isactive == true,
            status = certificate.Status,
            assignedDate = certificate.Createdat,
            expiryDate = certificate.Createdat?.AddYears(1),
            isExpired = isExpired,
            message = isValid ? "Certificate is valid" : "Certificate is not valid"
        };
    }
}