using EV_SCMMS.Core.Application.DTOs.User;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Manage user accounts
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserAccountController : ControllerBase
{
    private readonly IUserAccountService _userAccountService;

    public UserAccountController(IUserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userAccountService.GetByIdAsync(id);
        if (result.IsSuccess) return Ok(result.Data);
        return NotFound(result.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userAccountService.GetAllAsync();
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserAccountDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _userAccountService.CreateAsync(dto);
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.UserId }, result.Data);
        }
        return BadRequest(result.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserAccountDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _userAccountService.UpdateAsync(id, dto);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _userAccountService.UpdateRoleAsync(id, dto.RoleId);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userAccountService.DeleteAsync(id);
        if (result.IsSuccess) return Ok(new { message = "User account deleted successfully" });
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get all technicians with their certificate information
    /// </summary>
    /// <returns>List of technicians with their assigned certificates</returns>
    /// <remarks>
    /// Returns all users with "TECHNICIAN" role including:
    /// - userId: GUID - Technician's unique identifier
    /// - userName: string - Full name (e.g., "Kỹ thuật An Nguyễn Văn")
    /// - email: string - Email address (e.g., "tech1@ev.com")
    /// - phoneNumber: string - Phone number (e.g., "0910000001")
    /// - isActive: boolean - Account status
    /// - certificates: array - List of assigned certificates with:
    ///   - userCertificateId: GUID - Assignment ID
    ///   - certificateId: GUID - Certificate ID
    ///   - certificateName: string - Certificate name
    ///   - status: string - "Pending", "Approved", "Revoked"
    ///   - isActive: boolean - Assignment status
    /// - validCertificatesCount: int - Number of valid certificates
    /// - expiredCertificatesCount: int - Number of expired certificates
    /// 
    /// This endpoint is essential for:
    /// - Getting userId and certificateId for certificate management
    /// - Viewing technician certificate status overview
    /// - Certificate assignment planning
    /// </remarks>
    [HttpGet("technicians")]
    public async Task<IActionResult> GetAllTechnicians()
    {
        var result = await _userAccountService.GetAllTechniciansAsync();
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }
}