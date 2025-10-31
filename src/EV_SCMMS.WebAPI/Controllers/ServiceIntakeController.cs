using EV_SCMMS.Core.Application.DTOs.ServiceIntake;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// API endpoints for Service Intake lifecycle
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF,TECHNICIAN")]
public class ServiceIntakeController : ControllerBase
{
    private readonly IServiceIntakeService _serviceIntakeService;

    public ServiceIntakeController(IServiceIntakeService serviceIntakeService)
    {
        _serviceIntakeService = serviceIntakeService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceIntakeDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid user context");
        }

        var result = await _serviceIntakeService.CreateAsync(dto, userId, ct);
        if (result.IsSuccess && result.Data != null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceIntakeDto dto, CancellationToken ct)
    {
        var result = await _serviceIntakeService.UpdateAsync(id, dto, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}/verify")]
    public async Task<IActionResult> Verify(Guid id, CancellationToken ct)
    {
        var result = await _serviceIntakeService.VerifyAsync(id, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}/finalize")]
    public async Task<IActionResult> Finalize(Guid id, CancellationToken ct)
    {
        var result = await _serviceIntakeService.FinalizeAsync(id, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _serviceIntakeService.GetByIdAsync(id, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return NotFound(result.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetRange([FromQuery] Guid? centerId, [FromQuery] DateOnly? date, [FromQuery] string? status, [FromQuery] Guid? technicianId, CancellationToken ct)
    {
        var result = await _serviceIntakeService.GetRangeAsync(centerId, date, status, technicianId, ct);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var result = await _serviceIntakeService.CancelAsync(id, ct);
        if (result.IsSuccess) return Ok(new { success = true, message = result.Message ?? "Intake cancelled" });
        return BadRequest(result.Message);
    }
}
