using EV_SCMMS.Core.Application.DTOs.UserCertificate;

namespace EV_SCMMS.Core.Application.DTOs.User;

public class TechnicianWithCertificatesDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public List<UserCertificateDto> Certificates { get; set; } = new();
    public int ValidCertificatesCount { get; set; }
    public int ExpiredCertificatesCount { get; set; }
}