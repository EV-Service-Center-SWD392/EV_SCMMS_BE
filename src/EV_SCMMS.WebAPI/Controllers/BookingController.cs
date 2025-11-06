using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.Common;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using System.Security.Claims;

namespace EV_SCMMS.WebAPI.Controllers
{
  [ApiController]
  [Route("api/client/Booking")]
  [ServiceFilter(typeof(ValidationFilter))]
  [Authorize(Roles = "CUSTOMER, STAFF")]

  public class ClientBookingController : ControllerBase
  {
    private readonly BookingService _bookingService;

    public ClientBookingController(BookingService bookingService)
    {
      _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookingWithSlotDto>>> GetList([FromQuery] BookingQueryDto query)
    {
      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }

      var result = await _bookingService.GetBookingsAsync(customerId, query);
      if (!result.IsSuccess)
        return BadRequest(new { Message = result.Message });

      return Ok(result.Data);
    }

    [HttpGet("{bookingId}")]
    public async Task<ActionResult<PagedResult<Bookinghuykt>>> GetDetails(Guid bookingId)
    {
      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }
      var result = await _bookingService.GetBookingDetailsAsync(customerId, bookingId);
      if (!result.IsSuccess)
        return BadRequest(new { Message = result.Message });

      return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateBookingDto dto)
    {
      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }

      var result = await _bookingService.CreateBookingAsync(customerId, dto);
      if (!result.IsSuccess)
        return BadRequest(new { Message = result.Message });

      return CreatedAtAction(nameof(GetDetails), result.Data);
    }



    [HttpPut("{id}")]
    public async Task<ActionResult<Bookinghuykt>> Update(Guid id, [FromBody] UpdateBookingDto dto)
    {
      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }

      var result = await _bookingService.UpdateBookingAsync(customerId, id, dto);
      if (!result.IsSuccess)
        return BadRequest(new { Message = result.Message });

      return NoContent();
    }
  }
}
