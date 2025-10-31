using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.Interfaces.Services;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace EV_SCMMS.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptController : ControllerBase
{
  private readonly IReceiptService _receiptService;

  public ReceiptController(IReceiptService receiptService)
  {
    _receiptService = receiptService;
  }

  [HttpGet]
  [Authorize(Policy = "StaffAndAdmin")]

  public async Task<IActionResult> GetAll()
  {
    var result = await _receiptService.GetAllAsync();
    if (!result.IsSuccess) return BadRequest(result.Message);
    return Ok(result.Data);
  }

  [HttpGet("{id:guid}")]
  [Authorize]

  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _receiptService.GetByIdAsync(id);
    if (!result.IsSuccess) return NotFound(result.Message);
    return Ok(result.Data);
  }

  [HttpGet("customer/{customerId:guid}")]
  [Authorize(Policy ="StaffAndAdmin")]
  public async Task<IActionResult> GetAllByCustomerId(Guid customerId)
  {
    var result = await _receiptService.GetByCustomerIdAsync(customerId);
    if (!result.IsSuccess) return BadRequest(result.Message);
    return Ok(result.Data);
  }

  [HttpGet("me")]
  [Authorize]

  public async Task<IActionResult> GetForCurrentUser()
  {
    var result = await _receiptService.GetForCurrentUserAsync();
    if (!result.IsSuccess) return Unauthorized(result.Message);
    return Ok(result.Data);
  }
}
