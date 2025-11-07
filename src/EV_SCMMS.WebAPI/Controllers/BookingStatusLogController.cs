using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "CUSTOMER")]
public class BookingStatusLogsController : ControllerBase
{
  private readonly BookingStatusLogService _logService;

  public BookingStatusLogsController(BookingStatusLogService logService)
  {
    _logService = logService;
  }

  [HttpGet("customer")]
  public async Task<IActionResult> GetLogsByCustomerId([FromQuery] Guid bookingId)
  {
    var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      return Unauthorized("Invalid user identity");

    var logs = await _logService.GetLogsAsync(customerId, bookingId);
    return Ok(logs);
  }

  [HttpPatch("customer/notification/seen-all")]
  public async Task<IActionResult> UpdateSeenForALlLogs()
  {
    var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      return Unauthorized("Invalid user identity");

    var result = await _logService.UpdateSeenLogs(customerId);

    if (!result)
    {
      return Problem();
    }
    return Ok();
  }
}
