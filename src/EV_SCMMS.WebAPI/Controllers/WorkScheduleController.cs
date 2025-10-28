using EV_SCMMS.Core.Application.DTOs.WorkSchedule;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Manage technician work schedules and availability
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "ADMIN,STAFF,TECHNICIAN")] // Temporarily disabled for testing
public class WorkScheduleController : ControllerBase
{
    private readonly IWorkScheduleService _workScheduleService;

    public WorkScheduleController(IWorkScheduleService workScheduleService)
    {
        _workScheduleService = workScheduleService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _workScheduleService.GetByIdAsync(id);
        if (result.IsSuccess) return Ok(result.Data);
        return NotFound(result.Message);
    }

    [HttpGet("technician/{id}")]
    public async Task<IActionResult> GetByTechnician(Guid id)
    {
        var result = await _workScheduleService.GetByTechnicianIdAsync(id);
        if (result.IsSuccess) return Ok(result.Data);
        return NotFound(result.Message);
    }

    [HttpGet("range")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateOnly start, [FromQuery] DateOnly end, [FromQuery] Guid? technicianId)
    {
        var result = await _workScheduleService.GetByDateRangeAsync(start, end, technicianId);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable([FromQuery] DateOnly date, [FromQuery] TimeOnly startTime, [FromQuery] TimeOnly endTime)
    {
        var result = await _workScheduleService.GetAvailableTechniciansAsync(date, startTime, endTime);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkScheduleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _workScheduleService.CreateAsync(dto);
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.WorkScheduleId }, result.Data);
        }
        return BadRequest(result.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkScheduleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _workScheduleService.UpdateAsync(id, dto);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _workScheduleService.DeleteAsync(id);
        if (result.IsSuccess) return Ok(new { message = "Work schedule deleted successfully" });
        return BadRequest(result.Message);
    }
}

