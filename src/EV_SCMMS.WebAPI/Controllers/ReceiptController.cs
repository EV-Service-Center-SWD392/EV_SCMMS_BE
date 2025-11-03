using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.Receipt;
using EV_SCMMS.Core.Application.Interfaces.Services;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Controller for managing inventory operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReceiptController : ControllerBase
{
  private readonly IReceiptService _receiptService;

  public ReceiptController(IReceiptService receiptService)
  {
    _receiptService = receiptService;
  }

  /// <summary>
  /// Get all receipts
  /// </summary>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<ReceiptDto>), StatusCodes.Status200OK)]
  [Authorize(Policy = "StaffAndAdmin")]
  public async Task<IActionResult> GetAll()
  {
    var result = await _receiptService.GetAllAsync();

    if (result.IsSuccess)
    {
      return Ok(result.Data);
    }

    return BadRequest(result.Message);
  }

  /// <summary>
  /// Get receipt by ID
  /// </summary>
  [HttpGet("{id:guid}")]
  [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [Authorize]

  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _receiptService.GetByIdAsync(id);

    if (result.IsSuccess)
    {
      return Ok(result.Data);
    }

    return NotFound(result.Message);
  }

  /// <summary>
  /// Get receipts by customer ID
  /// </summary>
  [HttpGet("customer/{customerId:guid}")]
  [ProducesResponseType(typeof(IEnumerable<ReceiptDto>), StatusCodes.Status200OK)]
  [Authorize(Policy = "StaffAndAdmin")]
  public async Task<IActionResult> GetAllByCustomerId(Guid customerId)
  {
    var result = await _receiptService.GetByCustomerIdAsync(customerId);

    if (result.IsSuccess)
    {
      return Ok(result.Data);
    }

    return BadRequest(result.Message);
  }

  /// <summary>
  /// Get receipts for current user
  /// </summary>
  [HttpGet("me")]
  [ProducesResponseType(typeof(IEnumerable<ReceiptDto>), StatusCodes.Status200OK)]
  [Authorize]

  public async Task<IActionResult> GetForCurrentUser()
  {
    var result = await _receiptService.GetForCurrentUserAsync();
    if (!result.IsSuccess) return Unauthorized(result.Message);
    return Ok(result.Data);
  }
}
