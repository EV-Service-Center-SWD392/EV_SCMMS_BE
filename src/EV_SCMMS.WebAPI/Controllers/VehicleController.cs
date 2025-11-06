using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.Vehicle;
using EV_SCMMS.Core.Application.DTOs.Common;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Results;
using System.Security.Claims;
using EV_SCMMS.Infrastructure.Mappings;
using StackExchange.Redis;

namespace EV_SCMMS.WebAPI.Controllers
{
  [ApiController]
  [Route("api/client/Vehicle")]
  [Authorize(Roles = "CUSTOMER")]
  [ServiceFilter(typeof(ValidationFilter))]
  public class ClientVehiclesController : ControllerBase
  {
    private readonly IVehicleService _vehicleService;

    public ClientVehiclesController(IVehicleService vehicleService)
    {
      _vehicleService = vehicleService;
    }


    [HttpGet]
    public async Task<ActionResult<PagedResult<VehicleDto>>> GetList([FromQuery] VehicleQueryDto query)
    {

      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }
      var result = await _vehicleService.GetListVehiclesAsync(query, customerId);
      if (!result.IsSuccess)
        return BadRequest(result.Message);

      return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDetailDto>> GetDetails(Guid id)
    {
      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }
      var result = await _vehicleService.GetVehicleAsync(id, customerId);
      if (!result.IsSuccess)
        return NotFound(result.Message);

      return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<VehicleDto>> Create([FromBody] CreateVehicleDto dto, CancellationToken ct = default)
    {

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }


      var customerIdClaims = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaims, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }

      var result = await _vehicleService.CreateAsync(dto, customerId, ct);
      if (!result.IsSuccess)
        return BadRequest(result.Message);

      var responseDto = result.Data.ToDto();
      return CreatedAtAction(nameof(GetDetails), new { id = result.Data.Vehicleid }, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleDto>> Update(Guid id, [FromBody] UpdateVehicleDto dto, CancellationToken ct = default)
    {

      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }

      var result = await _vehicleService.UpdateAsync(id, dto, customerId, ct);
      if (!result.IsSuccess)
        return NotFound(result.Message);
      var responseDto = result.Data.ToDto();
      return Ok(responseDto);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }

      var result = await _vehicleService.DeleteAsync(id, customerId, ct);
      if (!result.IsSuccess)
        return NotFound(result.Message);

      return NoContent();
    }

  }
}
