using EV_SCMMS.Core.Application.DTOs.UserRole;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Controller for user role management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;

    public UserRoleController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    /// <summary>
    /// Get all user roles
    /// </summary>
    /// <returns>List of all user roles</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userRoleService.GetAllAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get active user roles only
    /// </summary>
    /// <returns>List of active user roles</returns>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var result = await _userRoleService.GetActiveRolesAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get user role by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>User role details</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userRoleService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a new user role
    /// </summary>
    /// <param name="dto">User role creation data</param>
    /// <returns>Created user role</returns>
    /// <remarks>
    /// Required fields:
    /// - Name: Role name (must be unique)
    /// 
    /// Optional fields:
    /// - Description: Role description
    /// - Status: Role status (default: "Active")
    /// 
    /// Common role names: "Admin", "Staff", "Technician", "Customer"
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRoleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userRoleService.CreateAsync(dto);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Data?.RoleId }, result) : BadRequest(result);
    }

    /// <summary>
    /// Update an existing user role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="dto">User role update data</param>
    /// <returns>Updated user role</returns>
    /// <remarks>
    /// Optional fields:
    /// - Name: Role name (must be unique if changed)
    /// - Description: Role description
    /// - Status: Role status
    /// - IsActive: Active status flag
    /// </remarks>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userRoleService.UpdateAsync(id, dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete a user role (soft delete)
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Deletion result</returns>
    /// <remarks>
    /// This performs a soft delete by setting IsActive = false.
    /// The role will no longer appear in active role lists but data is preserved.
    /// </remarks>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userRoleService.DeleteAsync(id);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}