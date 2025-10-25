using EV_SCMMS.Core.Application.DTOs.BookingApproval;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Staff endpoints for reviewing booking requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF")]
public class BookingApprovalController : ControllerBase
{
    private readonly IBookingApprovalService _bookingApprovalService;

    public BookingApprovalController(IBookingApprovalService bookingApprovalService)
    {
        _bookingApprovalService = bookingApprovalService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _bookingApprovalService.GetByIdAsync(id, ct);
        if (result.IsSuccess && result.Data != null)
        {
            return Ok(result.Data);
        }

        return NotFound(result.Message);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending([FromQuery] Guid? centerId, [FromQuery] DateOnly? date, CancellationToken ct)
    {
        var result = await _bookingApprovalService.GetPendingAsync(centerId, date, ct);
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.Message);
    }

    [HttpPost("approve")]
    public async Task<IActionResult> Approve([FromBody] ApproveBookingDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _bookingApprovalService.ApproveAsync(dto, ct);
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.Message);
    }

    [HttpPost("reject")]
    public async Task<IActionResult> Reject([FromBody] RejectBookingDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _bookingApprovalService.RejectAsync(dto, ct);
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.Message);
    }
}
