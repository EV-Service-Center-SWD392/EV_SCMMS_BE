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
  [Route("api/[controller]")]
  [Authorize(Roles = "CUSTOMER")]
  // [Authorize]  // Yêu cầu JWT token cho tất cả endpoints
  [ServiceFilter(typeof(ValidationFilter))]
  public class VehiclesController : ControllerBase
  {
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
      _vehicleService = vehicleService;
    }

    // GET: api/vehicles - Danh sách phân trang với query filters
    // Ví dụ: /api/vehicles?page=1&pageSize=10&status=ACTIVE&year=2023&fromDate=2025-01-01&toDate=2025-12-31
    [HttpGet]
    public async Task<ActionResult<PagedResult<VehicleDto>>> GetList([FromQuery] VehicleQueryDto query)
    {
      // FluentValidation auto-check query DTO (Page > 0, Status valid, FromDate <= ToDate, etc.)
      // Nếu fail, tự return BadRequest với errors
      ;
      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }

      Console.WriteLine("haha", customerIdClaim);

      var result = await _vehicleService.GetListVehiclesAsync(query);
      if (!result.IsSuccess)
        return BadRequest(result.Message);

      return Ok(result.Data);
    }

    // GET: api/vehicles/{id} - Details với nested Customer (UserAccount)
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDetailDto>> GetDetails(Guid id)
    {
      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }
      var result = await _vehicleService.GetVehicleAsync(id);
      if (!result.IsSuccess)
        return NotFound(result.Message);

      return Ok(result.Data);
    }

    // POST: api/vehicles - Tạo mới (CustomerId từ JWT)
    [HttpPost]
    public async Task<ActionResult<VehicleDto>> Create([FromBody] CreateVehicleDto dto, CancellationToken ct = default)
    {

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      // FluentValidation auto-check dto (ModelId required, LicensePlate not empty, Status ACTIVE/INACTIVE, etc.)


      var customerIdClaims = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaims, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }

      var result = await _vehicleService.CreateAsync(dto, customerId, ct);
      if (!result.IsSuccess)
        return BadRequest(result.Message);

      var responseDto = result.Data.ToDto();  // Map entity sang DTO cho response
      return CreatedAtAction(nameof(GetDetails), new { id = result.Data.Vehicleid }, responseDto);
    }

    // PUT: api/vehicles/{id} - Update (CustomerId từ JWT)
    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleDto>> Update(Guid id, [FromBody] UpdateVehicleDto dto, CancellationToken ct = default)
    {
      // FluentValidation auto-check dto (Status valid nếu set, Year range, etc.)

      var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
      {
        return Unauthorized("Invalid user identity");
      }

      var result = await _vehicleService.UpdateAsync(id, dto, customerId, ct);
      if (!result.IsSuccess)
        return NotFound(result.Message);  // Hoặc BadRequest nếu validation fail ở service

      var responseDto = result.Data.ToDto();  // Map entity sang DTO
      return Ok(responseDto);
    }

    // DELETE: api/vehicles/{id} - Xóa (CustomerId từ JWT, check ownership nếu cần)
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

      return NoContent();  // 204
    }

  }
}
