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
}