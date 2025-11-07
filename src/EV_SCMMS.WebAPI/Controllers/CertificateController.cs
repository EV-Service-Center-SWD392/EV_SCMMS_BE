using EV_SCMMS.Core.Application.DTOs.Certificate;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificateController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    public CertificateController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    /// <summary>
    /// Get all certificates in the system
    /// </summary>
    /// <returns>List of all certificates</returns>
    /// <remarks>
    /// Returns all certificates with the following fields:
    /// - certificateId: GUID - Unique identifier
    /// - name: string - Certificate name (e.g., "Chứng chỉ sửa chữa xe điện cấp 1")
    /// - description: string - Certificate description
    /// - status: string - Certificate status ("ACTIVE", "INACTIVE")
    /// - isActive: boolean - Active flag
    /// - createdAt: DateTime - Creation timestamp
    /// - updatedAt: DateTime - Last update timestamp
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _certificateService.GetAllAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get all active certificates
    /// </summary>
    /// <returns>List of active certificates only</returns>
    /// <remarks>
    /// Returns only certificates with isActive = true and status = "ACTIVE".
    /// Response fields same as GetAll but filtered for active certificates only.
    /// 
    /// Useful for:
    /// - Certificate assignment dropdowns
    /// - Available certificates for technicians
    /// </remarks>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var result = await _certificateService.GetActiveCertificatesAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get a specific certificate by ID
    /// </summary>
    /// <param name="id">GUID - Certificate ID to retrieve</param>
    /// <returns>Certificate details</returns>
    /// <remarks>
    /// Returns detailed information for a specific certificate:
    /// - certificateId: GUID - Unique identifier
    /// - name: string - Certificate name
    /// - description: string - Detailed description
    /// - status: string - Current status
    /// - isActive: boolean - Active flag
    /// - createdAt: DateTime - Creation timestamp
    /// - updatedAt: DateTime - Last update timestamp
    /// 
    /// Sample URL: GET /api/Certificate/16bc380d-5e7d-4c73-954c-81662e9f2678
    /// </remarks>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _certificateService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a new certificate
    /// </summary>
    /// <param name="dto">Certificate creation data</param>
    /// <returns>Created certificate information</returns>
    /// <remarks>
    /// Required fields to create a certificate:
    /// - name: string (Required) - Certificate name (e.g., "Chứng chỉ bảo dưỡng Tesla Model 3")
    /// - description: string (Optional) - Detailed description of the certificate
    /// 
    /// Status and timestamps are automatically set by the system:
    /// - status: Defaults to "ACTIVE"
    /// - isActive: Defaults to true
    /// - createdAt/updatedAt: Set automatically
    /// 
    /// Sample request:
    /// ```json
    /// {
    ///   "name": "Chứng chỉ bảo dưỡng VinFast VF8",
    ///   "description": "Chứng chỉ chuyên về bảo dưỡng và sửa chữa xe VinFast VF8"
    /// }
    /// ```
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCertificateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _certificateService.CreateAsync(dto);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Data?.CertificateId }, result) : BadRequest(result);
    }

    /// <summary>
    /// Update an existing certificate
    /// </summary>
    /// <param name="id">GUID - Certificate ID to update</param>
    /// <param name="dto">Certificate update data</param>
    /// <returns>Updated certificate information</returns>
    /// <remarks>
    /// Fields that can be updated:
    /// - name: string (Optional) - New certificate name
    /// - description: string (Optional) - New description
    /// - status: string (Optional) - New status ("ACTIVE", "INACTIVE")
    /// - isActive: boolean (Optional) - Active flag
    /// 
    /// Only provide fields you want to update. Null/empty fields will be ignored.
    /// updatedAt timestamp is automatically set.
    /// 
    /// Sample request:
    /// ```json
    /// {
    ///   "name": "Chứng chỉ bảo dưỡng VinFast VF8 - Cập nhật 2025",
    ///   "description": "Phiên bản cập nhật với quy trình mới",
    ///   "status": "ACTIVE"
    /// }
    /// ```
    /// 
    /// Sample URL: PUT /api/Certificate/16bc380d-5e7d-4c73-954c-81662e9f2678
    /// </remarks>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCertificateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _certificateService.UpdateAsync(id, dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete a certificate (soft delete)
    /// </summary>
    /// <param name="id">GUID - Certificate ID to delete</param>
    /// <returns>Success status</returns>
    /// <remarks>
    /// Performs soft delete by setting:
    /// - isActive: false
    /// - status: "INACTIVE"
    /// - updatedAt: current timestamp
    /// 
    /// The certificate record remains in database but becomes inactive.
    /// All user certificate assignments for this certificate will also be affected.
    /// 
    /// Returns:
    /// - Success: { "isSuccess": true, "message": "Certificate deleted successfully" }
    /// - Error: { "isSuccess": false, "message": "Error message" }
    /// 
    /// Sample URL: DELETE /api/Certificate/16bc380d-5e7d-4c73-954c-81662e9f2678
    /// </remarks>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _certificateService.DeleteAsync(id);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}