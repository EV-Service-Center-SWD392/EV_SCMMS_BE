namespace EV_SCMMS.Core.Application.DTOs.UserCertificate;

public class UserCertificateDto
{
    public Guid UserCertificateId { get; set; }
    public Guid UserId { get; set; }
    public Guid CertificateId { get; set; }
    public string? Status { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public string? UserName { get; set; }
    public string? CertificateName { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public int DaysUntilExpiry { get; set; }
}