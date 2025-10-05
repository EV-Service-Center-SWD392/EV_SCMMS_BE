using EV_SCMMS.Core.Application.DTOs;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllUsersAsync(cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get user by id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserByIdAsync(id, cancellationToken);
        
        if (!result.IsSuccess)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Create new user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserDto userDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.CreateUserAsync(userDto, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Update existing user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserDto userDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.UpdateUserAsync(id, userDto, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Delete user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteUserAsync(id, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }
}
