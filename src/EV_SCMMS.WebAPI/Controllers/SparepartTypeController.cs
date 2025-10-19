using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.SparepartType;
using EV_SCMMS.Core.Application.Interfaces.Services;

namespace EV_SCMMS.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SparepartTypeController : ControllerBase
{
    private readonly ISparepartTypeService _sparepartTypeService;

    public SparepartTypeController(ISparepartTypeService sparepartTypeService)
    {
        _sparepartTypeService = sparepartTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sparepartTypeService.GetAllAsync();
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sparepartTypeService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSparepartTypeDto createDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _sparepartTypeService.CreateAsync(createDto);
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.TypeId }, result.Data)
            : BadRequest(result.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSparepartTypeDto updateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _sparepartTypeService.UpdateAsync(id, updateDto);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _sparepartTypeService.DeleteAsync(id);
        return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
    }
}
