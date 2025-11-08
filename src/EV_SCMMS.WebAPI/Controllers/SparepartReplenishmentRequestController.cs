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

    /// <summary>
    /// Approve a replenishment request
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="approveDto">Approval details including approver ID and optional notes</param>
    /// <returns>Updated replenishment request</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/SparepartReplenishmentRequest/{id}/approve
    ///     {
    ///         "approvedBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "notes": "Approved for immediate procurement"
    ///     }
    /// 
    /// </remarks>
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveRequest(Guid id, [FromBody] ApproveRequestDto approveDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _replenishmentRequestService.ApproveRequestAsync(id, approveDto);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Reject a replenishment request
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="rejectDto">Rejection details including rejector ID, reason and optional notes</param>
    /// <returns>Updated replenishment request</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/SparepartReplenishmentRequest/{id}/reject
    ///     {
    ///         "rejectedBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "reason": "Budget constraints for this quarter",
    ///         "notes": "Please resubmit next quarter"
    ///     }
    /// 
    /// </remarks>
    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectRequest(Guid id, [FromBody] RejectRequestDto rejectDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _replenishmentRequestService.RejectRequestAsync(id, rejectDto);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

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