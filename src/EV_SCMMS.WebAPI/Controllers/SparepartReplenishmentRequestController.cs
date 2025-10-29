using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.SparepartReplenishmentRequest;
using EV_SCMMS.Core.Application.Interfaces.Services;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Controller for managing sparepart replenishment request operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SparepartReplenishmentRequestController : ControllerBase
{
    private readonly ISparepartReplenishmentRequestService _replenishmentRequestService;

    public SparepartReplenishmentRequestController(ISparepartReplenishmentRequestService replenishmentRequestService)
    {
        _replenishmentRequestService = replenishmentRequestService;
    }

    /// <summary>
    /// Get all replenishment requests with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of replenishment requests</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllReplenishmentRequests()
    {
        var result = await _replenishmentRequestService.GetAllAsync();

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get replenishment request by ID
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <returns>Replenishment request details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReplenishmentRequestById(Guid id)
    {
        var result = await _replenishmentRequestService.GetByIdAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return NotFound(result.Message);
    }

    /// <summary>
    /// Create new replenishment request
    /// </summary>
    /// <param name="createDto">Replenishment request creation data</param>
    /// <returns>Created replenishment request</returns>
    [HttpPost]
    public async Task<IActionResult> CreateReplenishmentRequest([FromBody] CreateSparepartReplenishmentRequestDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _replenishmentRequestService.CreateAsync(createDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetReplenishmentRequestById), new { id = result.Data.Id }, result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Update existing replenishment request
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="updateDto">Replenishment request update data</param>
    /// <returns>Updated replenishment request</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReplenishmentRequest(Guid id, [FromBody] UpdateSparepartReplenishmentRequestDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _replenishmentRequestService.UpdateAsync(id, updateDto);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Delete replenishment request (soft delete)
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReplenishmentRequest(Guid id)
    {
        var result = await _replenishmentRequestService.DeleteAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(new { message = "Replenishment request deleted successfully" });
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get requests by sparepart ID
    /// </summary>
    /// <param name="sparepartId">Sparepart ID</param>
    /// <returns>List of requests for the sparepart</returns>
    [HttpGet("sparepart/{sparepartId}")]
    public async Task<IActionResult> GetRequestsBySparepart(Guid sparepartId)
    {
        var result = await _replenishmentRequestService.GetBySparepartIdAsync(sparepartId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get requests by center ID
    /// </summary>
    /// <param name="centerId">Center ID</param>
    /// <returns>List of requests for the center</returns>
    [HttpGet("center/{centerId}")]
    public async Task<IActionResult> GetRequestsByCenter(Guid centerId)
    {
        var result = await _replenishmentRequestService.GetByCenterIdAsync(centerId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get requests by status
    /// </summary>
    /// <param name="status">Request status</param>
    /// <returns>List of requests with the specified status</returns>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetRequestsByStatus(string status)
    {
        var result = await _replenishmentRequestService.GetByStatusAsync(status);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    // TODO: Implement these methods in ISparepartReplenishmentRequestService
    /*
    /// <summary>
    /// Get requests by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of requests in the date range</returns>
    [HttpGet("date-range")]
    public async Task<IActionResult> GetRequestsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _sparepartReplenishmentRequestService.GetByDateRangeAsync(startDate, endDate);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get urgent requests
    /// </summary>
    /// <returns>List of urgent requests</returns>
    [HttpGet("urgent")]
    public async Task<IActionResult> GetUrgentRequests()
    {
        var result = await _sparepartReplenishmentRequestService.GetUrgentRequestsAsync();
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }
    */

    // TODO: Implement ApproveAsync and RejectRequestAsync in service
    /*
    /// <summary>
    /// Approve a replenishment request
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="approvedBy">User approving the request</param>
    /// <returns>Success or failure result</returns>
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveRequest(Guid id, [FromBody] string approvedBy)
    {
        var result = await _replenishmentRequestService.ApproveAsync(id.ToString(), approvedBy);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }
    */

    // TODO: Implement RejectRequestAsync in service
    /*
    /// <summary>
    /// Reject a replenishment request
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="rejectedBy">User rejecting the request</param>
    /// <param name="reason">Rejection reason</param>
    /// <returns>Success or failure result</returns>
    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectRequest(Guid id, [FromBody] RejectRequestDto rejectDto)
    {
        var result = await _sparepartReplenishmentRequestService.RejectRequestAsync(id.ToString(), rejectDto.RejectedBy, rejectDto.Reason);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }
    */

    // TODO: Implement GetUrgentRequestsAsync in service
    /*
    /// <summary>
    /// Get urgent requests
    /// </summary>
    /// <returns>List of urgent requests</returns>
    [HttpGet("urgent")]
    public async Task<IActionResult> GetUrgentRequests()
    {
        var result = await _replenishmentRequestService.GetUrgentRequestsAsync();
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }
    */

    /// <summary>
    /// Get pending requests
    /// </summary>
    /// <returns>List of pending requests</returns>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var result = await _replenishmentRequestService.GetPendingRequestsAsync();
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }
}