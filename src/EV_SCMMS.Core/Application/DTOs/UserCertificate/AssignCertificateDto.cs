using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.UserCertificate;

/// <summary>
/// DTO for assigning certificate to user
/// </summary>
public class AssignCertificateDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid CertificateId { get; set; }
    
    public string Status { get; set; } = "Active";
    public DateTime? ExpiryDate { get; set; }
}