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
    /// Reject a pending certificate assignment
    /// </summary>
    /// <param name="userCertificateId">User certificate assignment ID</param>
    /// <returns>Rejection result</returns>
    /// <remarks>
    /// Rejects a pending certificate assignment by setting:
    /// - status: "Rejected"
    /// - isActive: false
    /// - updatedAt: current timestamp
    /// 
    /// Only pending certificates can be rejected.
    /// Already approved or active certificates cannot be rejected.
    /// 
    /// Returns:
    /// - Success: { "isSuccess": true, "message": "Certificate rejected successfully" }
    /// - Error: { "isSuccess": false, "message": "Error message" }
    /// 
    /// Sample URL: POST /api/UserCertificate/8052c2aa-6899-4fb3-bc56-e3ce8bf16f68/reject
    /// </remarks>
    [HttpPost("{userCertificateId}/reject")]
    public async Task<IActionResult> RejectCertificate(Guid userCertificateId)
    {
        var result = await _userCertificateService.RejectCertificateAsync(userCertificateId);
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
    /// Revoke a certificate assignment from a user (soft delete)
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
    /// Set certificate status to REVOKED
    /// </summary>
    /// <param name="userCertificateId">User certificate assignment ID</param>
    /// <returns>Status update result</returns>
    /// <remarks>
    /// Changes certificate status to "REVOKED" and sets:
    /// - status: "REVOKED"
    /// - isActive: false
    /// - updatedAt: current timestamp
    /// 
    /// This is different from DELETE (soft delete) - this specifically sets REVOKED status.
    /// Use this when you want to track that a certificate was formally revoked.
    /// 
    /// Returns:
    /// - Success: { "isSuccess": true, "message": "Certificate status set to REVOKED successfully" }
    /// - Error: { "isSuccess": false, "message": "Error message" }
    /// 
    /// Sample URL: POST /api/UserCertificate/8052c2aa-6899-4fb3-bc56-e3ce8bf16f68/revoke
    /// </remarks>
    [HttpPost("{userCertificateId}/revoke")]
    public async Task<IActionResult> SetCertificateStatusRevoked(Guid userCertificateId)
    {
        var result = await _userCertificateService.SetCertificateStatusRevokedAsync(userCertificateId);
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

    /// <summary>
    /// Get all user certificate assignments in the system
    /// </summary>
    /// <returns>List of all user certificate assignments</returns>
    /// <remarks>
    /// Returns all user certificate assignments with complete information including:
    /// - userCertificateId: GUID - Assignment ID
    /// - userId: GUID - Technician's user ID  
    /// - userName: string - Technician's full name (e.g., "Kỹ thuật An Nguyễn Văn")
    /// - certificateId: GUID - Certificate ID
    /// - certificateName: string - Certificate name (e.g., "Chứng chỉ sửa chữa xe điện cấp 1")
    /// - status: string - Assignment status ("PENDING", "APPROVED", "REJECTED")
    /// - isActive: boolean - Assignment active status
    /// - createdAt: DateTime - When the assignment was created
    /// - updatedAt: DateTime - When the assignment was last updated
    /// - expiryDate: DateTime - When the certificate will expire
    /// - isExpired: boolean - Whether the certificate has expired
    /// - daysUntilExpiry: int - Days until expiry
    /// 
    /// Use cases:
    /// - Admin dashboard to view all certificate assignments
    /// - Certificate management overview
    /// - Audit and reporting purposes
    /// - Filter and search certificate assignments
    /// 
    /// Sample response:
    /// ```json
    /// {
    ///   "data": [
    ///     {
    ///       "userCertificateId": "5d276020-e2e2-464d-af47-415d6e4ebfde",
    ///       "userId": "f20f31c9-c84b-4baf-b9df-7e66c91b91fb",
    ///       "userName": "Kỹ thuật An Nguyễn Văn",
    ///       "certificateId": "16bc380d-5e7d-4c73-954c-81662e9f2678",
    ///       "certificateName": "Chứng chỉ sửa chữa xe điện cấp 1",
    ///       "status": "APPROVED",
    ///       "isActive": true,
    ///       "createdAt": "2025-11-07T02:50:39.543832",
    ///       "updatedAt": "2025-11-07T02:50:59.166145",
    ///       "expiryDate": "2026-11-07T02:50:39.543832",
    ///       "isExpired": false,
    ///       "daysUntilExpiry": 364
    ///     }
    ///   ],
    ///   "isSuccess": true
    /// }
    /// ```
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> GetAllUserCertificates()
    {
        var result = await _userCertificateService.GetAllUserCertificatesAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get all pending certificate assignments for staff review
    /// </summary>
    /// <returns>List of pending certificate assignments</returns>
    /// <remarks>
    /// Returns all certificate assignments with status "PENDING" that require staff approval or rejection.
    /// This endpoint is designed for staff/admin dashboard to review and manage pending certificate requests.
    /// 
    /// Response includes:
    /// - userCertificateId: GUID - Assignment ID for approve/reject actions
    /// - userId: GUID - Technician's user ID
    /// - userName: string - Technician's full name (e.g., "Kỹ thuật An Nguyễn Văn")
    /// - certificateId: GUID - Certificate ID
    /// - certificateName: string - Certificate name (e.g., "Chứng chỉ sửa chữa xe điện cấp 1")
    /// - status: "PENDING" - All returned items have pending status
    /// - createdAt: DateTime - When the assignment was requested
    /// - expiryDate: DateTime - When the certificate will expire (if approved)
    /// - daysUntilExpiry: int - Days until expiry (364 for new assignments)
    /// 
    /// Use cases:
    /// - Staff dashboard to show pending certificate requests
    /// - Bulk approval/rejection workflows
    /// - Certificate request management
    /// </remarks>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingCertificates()
    {
        var result = await _userCertificateService.GetPendingCertificatesAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}