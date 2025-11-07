using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EV_SCMMS.Core.Application.DTOs.Certificate;

public class CreateCertificateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    public IFormFile? ImageFile { get; set; }
}