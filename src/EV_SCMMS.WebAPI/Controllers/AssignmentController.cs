using EV_SCMMS.Core.Application.DTOs.Assignment;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Manage technician assignments
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF,TECHNICIAN")]
public class AssignmentController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _assignmentService.CreateAsync(dto, ct);
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
        return BadRequest(result.Message);
    }

    [HttpPut("{id}/reschedule")]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleAssignmentDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _assignmentService.RescheduleAsync(id, dto, ct);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id}/reassign")]
    public async Task<IActionResult> Reassign(Guid id, [FromBody] ReassignTechnicianDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _assignmentService.ReassignAsync(id, dto, ct);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Cancel an assignment. Returns booking availability status for reassignment.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var result = await _assignmentService.CancelAsync(id, ct);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id}/noshow")]
    public async Task<IActionResult> NoShow(Guid id, CancellationToken ct)
    {
        var result = await _assignmentService.NoShowAsync(id, ct);
        if (result.IsSuccess) return Ok(new { success = true, message = "Assignment marked as no-show" });
        return BadRequest(result.Message);
    }

    [HttpGet("range")]
    public async Task<IActionResult> GetRange([FromQuery] Guid? centerId, [FromQuery] DateOnly? date, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await _assignmentService.GetRangeAsync(centerId, date, status, ct);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _assignmentService.GetByIdAsync(id, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return NotFound(result.Message);
    }
}
