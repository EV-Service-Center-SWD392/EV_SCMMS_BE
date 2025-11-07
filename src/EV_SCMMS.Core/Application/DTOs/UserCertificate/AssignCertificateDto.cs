using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.UserCertificate;

public class AssignCertificateDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid CertificateId { get; set; }
    
    public string Status { get; set; } = "ACTIVE";
}