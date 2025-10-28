using EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Manage user work schedule assignments
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "ADMIN,STAFF")] // Temporarily disabled for testing
public class UserWorkScheduleController : ControllerBase
{
    private readonly IUserWorkScheduleService _userWorkScheduleService;

    public UserWorkScheduleController(IUserWorkScheduleService userWorkScheduleService)
    {
        _userWorkScheduleService = userWorkScheduleService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userWorkScheduleService.GetByIdAsync(id);
        if (result.IsSuccess) return Ok(result.Data);
        return NotFound(result.Message);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var result = await _userWorkScheduleService.GetByUserIdAsync(userId);
        if (result.IsSuccess) return Ok(result.Data);
        return NotFound(result.Message);
    }

    [HttpGet("workschedule/{workScheduleId}")]
    public async Task<IActionResult> GetByWorkScheduleId(Guid workScheduleId)
    {
        var result = await _userWorkScheduleService.GetByWorkScheduleIdAsync(workScheduleId);
        if (result.IsSuccess) return Ok(result.Data);
        return NotFound(result.Message);
    }

    [HttpGet("user/{userId}/range")]
    public async Task<IActionResult> GetByDateRange(Guid userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _userWorkScheduleService.GetByDateRangeAsync(userId, startDate, endDate);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpGet("availability")]
    public async Task<IActionResult> CheckAvailability([FromQuery] Guid userId, [FromQuery] Guid workScheduleId)
    {
        var result = await _userWorkScheduleService.IsUserAvailableAsync(userId, workScheduleId);
        if (result.IsSuccess) return Ok(new { isAvailable = result.Data });
        return BadRequest(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserWorkScheduleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _userWorkScheduleService.CreateAsync(dto);
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.UserWorkScheduleId }, result.Data);
        }
        return BadRequest(result.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserWorkScheduleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _userWorkScheduleService.UpdateAsync(id, dto);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userWorkScheduleService.DeleteAsync(id);
        if (result.IsSuccess) return Ok(new { message = "User work schedule deleted successfully" });
        return BadRequest(result.Message);
    }

    [HttpPost("bulk-assign")]
    public async Task<IActionResult> BulkAssign([FromBody] BulkAssignTechniciansDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _userWorkScheduleService.BulkAssignAsync(dto);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPost("auto-assign")]
    public async Task<IActionResult> AutoAssign([FromBody] AutoAssignRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _userWorkScheduleService.AutoAssignAsync(dto);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpGet("conflicts/{userId}")]
    public async Task<IActionResult> GetConflicts(Guid userId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        var result = await _userWorkScheduleService.GetConflictingAssignmentsAsync(userId, startTime, endTime);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpGet("workload/{userId}")]
    public async Task<IActionResult> GetWorkload(Guid userId, [FromQuery] DateTime date)
    {
        var result = await _userWorkScheduleService.GetTechnicianWorkloadAsync(userId, date);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }
}