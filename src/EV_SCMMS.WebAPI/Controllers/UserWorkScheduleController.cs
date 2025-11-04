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

    /// <summary>
    /// Get user work schedules within a date range
    /// </summary>
    /// <param name="userId">User/technician ID</param>
    /// <param name="startDate">Start date (YYYY-MM-DD)</param>
    /// <param name="endDate">End date (YYYY-MM-DD)</param>
    /// <returns>List of work schedules in the date range</returns>
    /// <remarks>
    /// Useful for calendar views and schedule planning.
    /// Returns all shifts (Morning, Evening, Night) for the user in the specified period.
    /// </remarks>
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

    /// <summary>
    /// Create a new user work schedule assignment
    /// </summary>
    /// <param name="dto">User work schedule data</param>
    /// <returns>Created user work schedule</returns>
    /// <remarks>
    /// Required fields:
    /// - UserId: GUID of the technician/user
    /// - CenterName: Name of the service center (e.g., "EV Service - Quận 1")
    /// - Shift: Must be one of: "Morning", "Evening", "Night"
    /// - WorkDate: Date for the work schedule (YYYY-MM-DD format)
    /// 
    /// Shift times:
    /// - Morning: 07:00 - 12:00
    /// - Evening: 13:00 - 19:00
    /// - Night: 20:00 - 06:00 (next day)
    /// </remarks>
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

    /// <summary>
    /// Update an existing user work schedule assignment
    /// </summary>
    /// <param name="id">User work schedule ID</param>
    /// <param name="dto">Update data</param>
    /// <returns>Updated user work schedule</returns>
    /// <remarks>
    /// Optional fields:
    /// - Status: Assignment status (e.g., "Active", "Cancelled", "Completed")
    /// - IsActive: Boolean flag for soft delete
    /// </remarks>
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

    /// <summary>
    /// Bulk assign multiple technicians to the same work schedule
    /// </summary>
    /// <param name="dto">Bulk assignment data</param>
    /// <returns>Assignment results with success/failure details</returns>
    /// <remarks>
    /// Required fields:
    /// - CenterName: Name of the service center (e.g., "EV Service - Quận 1")
    /// - Shift: Must be one of: "Morning", "Evening", "Night"
    /// - WorkDate: Date for the work schedule (YYYY-MM-DD format)
    /// - TechnicianIds: Array of technician GUIDs to assign
    /// 
    /// Shift times:
    /// - Morning: 07:00 - 12:00
    /// - Evening: 13:00 - 19:00
    /// - Night: 20:00 - 06:00 (next day)
    /// 
    /// Returns:
    /// - successfulAssignments: List of successfully assigned technicians
    /// - failedAssignments: List of failed assignments with error details
    /// - totalProcessed, successCount, failureCount: Summary statistics
    /// </remarks>
    [HttpPost("bulk-assign")]
    public async Task<IActionResult> BulkAssign([FromBody] BulkAssignTechniciansDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _userWorkScheduleService.BulkAssignAsync(dto);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Automatically assign available technicians to a work schedule
    /// </summary>
    /// <param name="dto">Auto assignment request data</param>
    /// <returns>Assignment results with success/failure details</returns>
    /// <remarks>
    /// Required fields:
    /// - CenterName: Name of the service center (e.g., "EV Service - Quận 1")
    /// - Shift: Must be one of: "Morning", "Evening", "Night"
    /// - WorkDate: Date for the work schedule (YYYY-MM-DD format)
    /// - RequiredTechnicianCount: Number of technicians needed (1-50)
    /// 
    /// Optional fields:
    /// - RequiredSkills: Array of required skill names (not implemented yet)
    /// 
    /// Shift times:
    /// - Morning: 07:00 - 12:00
    /// - Evening: 13:00 - 19:00
    /// - Night: 20:00 - 06:00 (next day)
    /// 
    /// System will automatically find and assign available technicians.
    /// </remarks>
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

    /// <summary>
    /// Get all technicians with their assigned work schedules
    /// </summary>
    /// <returns>List of technicians with their work schedule assignments</returns>
    /// <remarks>
    /// Returns a comprehensive view of all technicians and their assigned work schedules.
    /// Each technician entry includes:
    /// - User information (name, email, phone)
    /// - List of assigned schedules with time, center, and status
    /// 
    /// Useful for:
    /// - Staff management dashboard
    /// - Schedule overview
    /// - Workload distribution analysis
    /// </remarks>
    [HttpGet("technicians-schedules")]
    public async Task<IActionResult> GetAllTechniciansWithSchedules()
    {
        var result = await _userWorkScheduleService.GetAllTechniciansWithSchedulesAsync();
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }
}