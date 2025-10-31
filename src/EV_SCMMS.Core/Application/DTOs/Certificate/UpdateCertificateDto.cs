namespace EV_SCMMS.Core.Application.DTOs.Certificate;

public class UpdateCertificateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public bool IsActive { get; set; } = true;
}