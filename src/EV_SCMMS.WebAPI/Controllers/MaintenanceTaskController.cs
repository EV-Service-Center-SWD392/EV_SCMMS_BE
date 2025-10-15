using EV_SCMMS.Core.Application.DTOs.MaintenanceTaskDungVmDto;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceTaskController : Controller
    {
        private readonly IMaintenanceTaskDungVmService _taskService;
        public MaintenanceTaskController(IMaintenanceTaskDungVmService taskService)
        {
            _taskService = taskService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _taskService.GetAllAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _taskService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MaintenanceTaskDungVmCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _taskService.CreateAsync(createDto);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data.MaintenanceTaskId }, result);
            }
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MaintenanceTaskDungVmUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _taskService.UpdateAsync(id, updateDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")] 
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _taskService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }





    }
}
