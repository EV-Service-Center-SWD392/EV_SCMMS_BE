using EV_SCMMS.Core.Application.DTOs.Payment;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;

namespace EV_SCMMS.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class TransactionController : ControllerBase
{
  private readonly ITransactionService _transactionService;

  public TransactionController(ITransactionService transactionService)
  {
    _transactionService = transactionService;
  }

  [HttpPost]
  [Authorize]
  public async Task<IActionResult> Create([FromBody] CreateTransactionDto createDto)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var result = await _transactionService.CreateAsync(createDto);
    if (result.IsSuccess) return CreatedAtAction(nameof(GetById), new { id = result.Data.TransactionId }, result.Data);
    return BadRequest(result.Message);
  }

  [HttpGet("{id}")]
  [Authorize]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _transactionService.GetByIdAsync(id);
    if (result.IsSuccess) return Ok(result.Data);
    return NotFound(result.Message);
  }

  [HttpPut("{id}")]
  [Authorize(policy: "StaffAndAdmin")]
  public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTransactionStatusDto updateDto)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var result = await _transactionService.UpdateStatusAsync(id, updateDto);
    if (result.IsSuccess) return Ok(result.Data);
    return BadRequest(result.Message);
  }

  [HttpDelete("{id}")]
  [Authorize]
  public async Task<IActionResult> Delete(Guid id)
  {
    var result = await _transactionService.DeleteAsync(id);
    if (result.IsSuccess) return Ok(new { message = "Transaction cancelled" });
    return BadRequest(result.Message);
  }
   
  [HttpGet("{userId}/user")]
  [Authorize]
  public async Task<IActionResult> GetByUserId(Guid userId)
  {
    var result = await _transactionService.GetAllByUserIdAsync(userId);
    if (result.IsSuccess) return Ok(result.Data);
    return NotFound(result.Message);
  }

  [HttpGet("{orderId}/order")]
  [Authorize]
  public async Task<IActionResult> GetAllByOrderId(Guid orderId)
  {
    var result = await _transactionService.GetAllByOrderIdAsync(orderId);
    if (result.IsSuccess) return Ok(result.Data);
    return NotFound(result.Message);
  }

  [HttpGet()]
  [Authorize(policy: "StaffAndAdmin")]
  public async Task<IActionResult> GetAll()
  {
    var result = await _transactionService.GetAllAsync();
    if (result.IsSuccess) return Ok(result.Data);
    return NotFound(result.Message);
  }

  // New: endpoint to receive PayOS webhook (moved from PaymentController)
  [HttpPost("payos_transfer_handler")]
  public async Task<IActionResult> PayOsTransferHandler([FromBody] WebhookType body)
  {
    var result = await _transactionService.HandlePayOsWebhookAsync(body);
    return Ok(new Response(0, "ok", null));

    //return BadRequest(new Response( -1, "fail", null ));
  }

  // New: endpoint to confirm webhook URL with PayOS (moved from OrderController)
  [HttpPost("confirm-webhook")]
  public async Task<IActionResult> ConfirmWebhook([FromBody] ConfirmWebhook body)
  {
    var result = await _transactionService.ConfirmWebhookAsync(body.webhook_url);
    if (result.IsSuccess) return Ok(new { success = true });
    return BadRequest(new { success = false, error = result.Message });
  }

  // New: cancel payment link via transaction service
  [HttpPut("cancel/{orderCode:int}")]
  public async Task<IActionResult> CancelPaymentLink([FromRoute] int orderCode)
  {
    var result = await _transactionService.CancelPayOsPaymentLinkAsync(orderCode);
    if (result.IsSuccess) return Ok(new { success = true });
    return BadRequest(new { success = false, error = result.Message });
  }
}
