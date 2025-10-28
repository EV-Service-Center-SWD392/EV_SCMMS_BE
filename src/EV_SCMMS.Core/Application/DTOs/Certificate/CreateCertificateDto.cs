using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Certificate;

/// <summary>
/// DTO for creating Certificate
/// </summary>
public class CreateCertificateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    public string Status { get; set; } = "Active";
}