using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.SparepartForecast;
using EV_SCMMS.Core.Application.Interfaces.Services;

namespace EV_SCMMS.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SparepartForecastController : ControllerBase
{
    private readonly ISparepartForecastService _sparepartForecastService;

    public SparepartForecastController(ISparepartForecastService sparepartForecastService)
    {
        _sparepartForecastService = sparepartForecastService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sparepartForecastService.GetAllAsync();
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sparepartForecastService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSparepartForecastDto createDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _sparepartForecastService.CreateAsync(createDto);
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.ForecastId }, result.Data)
            : BadRequest(result.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSparepartForecastDto updateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _sparepartForecastService.UpdateAsync(id, updateDto);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _sparepartForecastService.DeleteAsync(id);
        return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
    }
}
