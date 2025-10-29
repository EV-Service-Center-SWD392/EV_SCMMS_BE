using EV_SCMMS.Core.Application.DTOs.UserCertificate;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Controller for user certificate management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserCertificateController : ControllerBase
{
    private readonly IUserCertificateService _userCertificateService;

    public UserCertificateController(IUserCertificateService userCertificateService)
    {
        _userCertificateService = userCertificateService;
    }

    /// <summary>
    /// Assign certificate to user
    /// </summary>
    [HttpPost("assign")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AssignCertificate([FromBody] AssignCertificateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userCertificateService.AssignCertificateAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Revoke user certificate
    /// </summary>
    [HttpPost("{userCertificateId:guid}/revoke")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RevokeCertificate(Guid userCertificateId)
    {
        var result = await _userCertificateService.RevokeCertificateAsync(userCertificateId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get certificates for specific user
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserCertificates(Guid userId)
    {
        var result = await _userCertificateService.GetUserCertificatesAsync(userId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get all users who have specific certificate
    /// </summary>
    [HttpGet("certificate/{certificateId:guid}/holders")]
    public async Task<IActionResult> GetCertificateHolders(Guid certificateId)
    {
        var result = await _userCertificateService.GetCertificateHoldersAsync(certificateId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get certificates expiring within specified days
    /// </summary>
    [HttpGet("expiring")]
    public async Task<IActionResult> GetExpiringCertificates([FromQuery] int daysAhead = 30)
    {
        var result = await _userCertificateService.GetExpiringCertificatesAsync(daysAhead);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Check if user has specific certificate
    /// </summary>
    [HttpGet("check/{userId:guid}/{certificateId:guid}")]
    public async Task<IActionResult> HasCertificate(Guid userId, Guid certificateId)
    {
        var result = await _userCertificateService.HasCertificateAsync(userId, certificateId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}