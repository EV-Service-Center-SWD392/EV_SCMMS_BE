using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.DTOs.VehicleConditionDungVm;

namespace EV_SCMMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleConditionController : Controller
    {
        private readonly IVehicleConditionDungVmService _vehicleConditionService;
        public VehicleConditionController(IVehicleConditionDungVmService vehicleConditionService)
        {
            _vehicleConditionService = vehicleConditionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _vehicleConditionService.GetAllAsync();
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _vehicleConditionService.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleConditionDungVmCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var result = await _vehicleConditionService.CreateAsync(createDto);
            return result.IsSuccess
               ? CreatedAtAction(nameof(GetById), new { id = result.Data!.VehicleConditionDungVmid }, result.Data)
               : BadRequest(result.Message);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VehicleConditionDungVmUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _vehicleConditionService.UpdateAsync(id, updateDto);

            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _vehicleConditionService.DeleteAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }



    }
}


