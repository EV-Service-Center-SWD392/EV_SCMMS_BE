using EV_SCMMS.Core.Application.DTOs.UserCertificate;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Certificate management for technicians
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserCertificateController : ControllerBase
{
    private readonly IUserCertificateService _userCertificateService;

    public UserCertificateController(IUserCertificateService userCertificateService)
    {
        _userCertificateService = userCertificateService;
    }

    /// <summary>
    /// Assign a certificate to a technician
    /// </summary>
    /// <param name="dto">Certificate assignment details</param>
    /// <returns>Assignment result with userCertificateId</returns>
    [HttpPost("assign")]
    public async Task<IActionResult> AssignCertificate([FromBody] AssignCertificateDto dto)
    {
        var result = await _userCertificateService.AssignCertificateAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Approve a pending certificate assignment
    /// </summary>
    /// <param name="userCertificateId">User certificate assignment ID</param>
    /// <returns>Approval result</returns>
    [HttpPost("{userCertificateId}/approve")]
    public async Task<IActionResult> ApproveCertificate(Guid userCertificateId)
    {
        var result = await _userCertificateService.ApproveCertificateAsync(userCertificateId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get expiry status for a specific certificate
    /// </summary>
    /// <param name="certificateId">Certificate ID</param>
    /// <returns>Expiry status information</returns>
    [HttpGet("{certificateId}/expiry-status")]
    public async Task<IActionResult> GetCertificateExpiryStatus(Guid certificateId)
    {
        var result = await _userCertificateService.GetCertificateExpiryStatusAsync(certificateId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Validate if a user has a specific certificate and it's active
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="certificateId">Certificate ID</param>
    /// <returns>Validation result</returns>
    [HttpGet("validate/{userId}/{certificateId}")]
    public async Task<IActionResult> ValidateUserCertificate(Guid userId, Guid certificateId)
    {
        var result = await _userCertificateService.ValidateUserCertificateAsync(userId, certificateId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get all certificates assigned to a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's certificates with status</returns>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserCertificates(Guid userId)
    {
        var result = await _userCertificateService.GetUserCertificatesAsync(userId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get all users who hold a specific certificate
    /// </summary>
    /// <param name="certificateId">Certificate ID</param>
    /// <returns>List of certificate holders</returns>
    [HttpGet("certificate/{certificateId}/holders")]
    public async Task<IActionResult> GetCertificateHolders(Guid certificateId)
    {
        var result = await _userCertificateService.GetCertificateHoldersAsync(certificateId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Revoke a certificate assignment from a user
    /// </summary>
    /// <param name="userCertificateId">User certificate assignment ID</param>
    /// <returns>Revocation result</returns>
    [HttpDelete("{userCertificateId}")]
    public async Task<IActionResult> RevokeCertificate(Guid userCertificateId)
    {
        var result = await _userCertificateService.RevokeCertificateAsync(userCertificateId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get certificates that are expiring within specified days
    /// </summary>
    /// <param name="daysAhead">Number of days to look ahead (default: 30)</param>
    /// <returns>List of expiring certificates</returns>
    [HttpGet("expiring")]
    public async Task<IActionResult> GetExpiringCertificates([FromQuery] int daysAhead = 30)
    {
        var result = await _userCertificateService.GetExpiringCertificatesAsync(daysAhead);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}