using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.Common;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Results;
using System.Globalization;

namespace EV_SCMMS.WebAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [ServiceFilter(typeof(ValidationFilter))]
  public class BookingSchedulesController : ControllerBase
  {
    private readonly BookingScheduleService _bookingScheduleService;

    public BookingSchedulesController(BookingScheduleService bookingScheduleService)
    {
      _bookingScheduleService = bookingScheduleService;
    }

    [HttpGet("pattern/{centerId}")]
    [AllowAnonymous]
    public async Task<ActionResult<CenterSchedulesResponseShorten>> GetByCenter(Guid centerId)
    {
      var result = await _bookingScheduleService.GetCenterSchedulesAsync(centerId);
      return Ok(result);
    }

    [HttpGet("client/{centerId}")]
    public async Task<ActionResult<CenterSchedulesResponse>> GetByCenter(Guid centerId, [FromQuery] CenterSchedulesQueryDto query)
    {

      if (!DateOnly.TryParse(query.StartDate, CultureInfo.InvariantCulture, out var startDate) ||
          !DateOnly.TryParse(query.EndDate, CultureInfo.InvariantCulture, out var endDate))
      {
        return BadRequest("Invalid date format in query");
      }

      var result = await _bookingScheduleService.GetBookingSchedulesByCenter(centerId, startDate, endDate);
      if (!result.IsSuccess)
        return BadRequest(result.Message);

      return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Upsert([FromBody] BookingScheduleDto dto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var result = await _bookingScheduleService.UpsertBookingSchedulesByCenterAsync(dto);

      if (!result.IsSuccess)
        return Conflict(result.Message);

      return NoContent();
    }
  }
}
