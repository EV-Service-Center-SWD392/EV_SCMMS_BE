using EV_SCMMS.Core.Application.DTOs.Certificate;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificateController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    public CertificateController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _certificateService.GetAllAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var result = await _certificateService.GetActiveCertificatesAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _certificateService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCertificateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _certificateService.CreateAsync(dto);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Data?.CertificateId }, result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCertificateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _certificateService.UpdateAsync(id, dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _certificateService.DeleteAsync(id);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}