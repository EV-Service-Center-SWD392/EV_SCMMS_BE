using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.SparepartUsageHistory;
using EV_SCMMS.Core.Application.Interfaces.Services;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Controller for managing sparepart usage history operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SparepartUsageHistoryController : ControllerBase
{
    private readonly ISparepartUsageHistoryService _usageHistoryService;

    public SparepartUsageHistoryController(ISparepartUsageHistoryService usageHistoryService)
    {
        _usageHistoryService = usageHistoryService;
    }

    /// <summary>
    /// Get all usage histories with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of usage histories</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllUsageHistories()
    {
        var result = await _usageHistoryService.GetAllAsync();

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get usage history by ID
    /// </summary>
    /// <param name="id">Usage history ID</param>
    /// <returns>Usage history details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsageHistoryById(Guid id)
    {
        var result = await _usageHistoryService.GetByIdAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return NotFound(result.Message);
    }

    /// <summary>
    /// Create new usage history
    /// </summary>
    /// <param name="createDto">Usage history creation data</param>
    /// <returns>Created usage history</returns>
    [HttpPost]
    public async Task<IActionResult> CreateUsageHistory([FromBody] CreateSparepartUsageHistoryDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _usageHistoryService.CreateAsync(createDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetUsageHistoryById), new { id = result.Data.Id }, result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Update existing usage history
    /// </summary>
    /// <param name="id">Usage history ID</param>
    /// <param name="updateDto">Usage history update data</param>
    /// <returns>Updated usage history</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUsageHistory(Guid id, [FromBody] UpdateSparepartUsageHistoryDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _usageHistoryService.UpdateAsync(id, updateDto);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Delete usage history (soft delete)
    /// </summary>
    /// <param name="id">Usage history ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsageHistory(Guid id)
    {
        var result = await _usageHistoryService.DeleteAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(new { message = "Usage history deleted successfully" });
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get usage histories by sparepart ID
    /// </summary>
    /// <param name="sparepartId">Sparepart ID</param>
    /// <returns>List of usage histories for the sparepart</returns>
    [HttpGet("sparepart/{sparepartId}")]
    public async Task<IActionResult> GetUsageHistoriesBySparepart(Guid sparepartId)
    {
        var result = await _usageHistoryService.GetBySparepartIdAsync(sparepartId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get usage histories by center ID
    /// </summary>
    /// <param name="centerId">Center ID</param>
    /// <returns>List of usage histories for the center</returns>
    [HttpGet("center/{centerId}")]
    public async Task<IActionResult> GetUsageHistoriesByCenter(Guid centerId)
    {
        var result = await _usageHistoryService.GetByCenterIdAsync(centerId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get usage histories by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of usage histories in the date range</returns>
    [HttpGet("date-range")]
    public async Task<IActionResult> GetUsageHistoriesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _usageHistoryService.GetByDateRangeAsync(startDate, endDate);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get usage histories by vehicle ID
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <returns>List of usage histories for the vehicle</returns>
    [HttpGet("vehicle/{vehicleId}")]
    public async Task<IActionResult> GetUsageHistoriesByVehicle(string vehicleId)
    {
        var result = await _usageHistoryService.GetByVehicleIdAsync(vehicleId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get usage statistics for a sparepart
    /// </summary>
    /// <param name="sparepartId">Sparepart ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Usage statistics</returns>
    [HttpGet("statistics/{sparepartId}")]
    public async Task<IActionResult> GetUsageStatistics(Guid sparepartId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _usageHistoryService.GetUsageStatisticsAsync(sparepartId, startDate, endDate);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }
}