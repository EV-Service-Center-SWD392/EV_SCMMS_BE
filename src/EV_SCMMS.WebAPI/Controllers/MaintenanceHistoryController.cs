using EV_SCMMS.Core.Application.DTOs.MaintenanceHistoryDungVmDto;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceHistoryController : Controller
    {
        private readonly IMaintenanceHistoryDungVmService _historyService;
        public MaintenanceHistoryController(IMaintenanceHistoryDungVmService historyService)
        {
            _historyService = historyService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _historyService.GetAllAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _historyService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MaintenanceHistoryDungVmCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _historyService.CreateAsync(createDto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data!.MaintenanceHistoryDungVmid }, result);
            }
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MaintenanceHistoryDungVmUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _historyService.UpdateAsync(id, updateDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _historyService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetByVehicleId(Guid vehicleId)
        {
            var result = await _historyService.GetByVehicleIdAsync(vehicleId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return NotFound(result);
        }



    }
}
