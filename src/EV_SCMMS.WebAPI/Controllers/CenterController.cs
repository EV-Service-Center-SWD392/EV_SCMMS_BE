using EV_SCMMS.Core.Application.DTOs.Center;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Manage service centers
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CenterController : ControllerBase
{
    private readonly ICenterService _centerService;

    public CenterController(ICenterService centerService)
    {
        _centerService = centerService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _centerService.GetByIdAsync(id);
        if (result.IsSuccess) return Ok(result.Data);
        return NotFound(result.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _centerService.GetAllAsync(); // Get first 100 centers
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCenterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _centerService.CreateAsync(dto);
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.CenterId }, result.Data);
        }
        return BadRequest(result.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCenterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _centerService.UpdateAsync(id, dto);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _centerService.DeleteAsync(id);
        if (result.IsSuccess) return Ok(new { message = "Center deleted successfully" });
        return BadRequest(result.Message);
    }
}
