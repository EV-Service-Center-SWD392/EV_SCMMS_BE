namespace EV_SCMMS.Core.Application.DTOs.Certificate;

/// <summary>
/// DTO for Certificate entity
/// </summary>
public class CertificateDto
{
    public Guid CertificateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Status { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}