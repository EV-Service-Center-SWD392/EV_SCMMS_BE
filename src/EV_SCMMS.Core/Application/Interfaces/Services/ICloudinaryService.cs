using Microsoft.AspNetCore.Http;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string folder = "certificates");
    Task<bool> DeleteImageAsync(string publicId);
}