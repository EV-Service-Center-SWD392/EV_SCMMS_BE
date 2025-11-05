using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.Vehicle;
using EV_SCMMS.Core.Application.DTOs.Common;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Results;
using System.Security.Claims;
using EV_SCMMS.Infrastructure.Mappings;
using StackExchange.Redis;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.WebAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  // [Authorize]  // Yêu cầu JWT token cho tất cả endpoints
  public class VehicleModelController : ControllerBase
  {
    private readonly IVehicleService _vehicleService;

    public VehicleModelController(IVehicleService vehicleService)
    {
      _vehicleService = vehicleService;
    }


    [HttpGet]
    public async Task<ActionResult<List<Vehiclemodel>>> GetList()
    {

      var result = await _vehicleService.GetListVehicleModel();
      if (!result.IsSuccess)
        return BadRequest(result.Message);

      return Ok(result.Data);
    }


  }
}
