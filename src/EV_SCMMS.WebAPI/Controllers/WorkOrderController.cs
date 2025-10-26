using EV_SCMMS.Core.Application.DTOs.WorkOrder;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// API endpoints for Work Order lifecycle (Technician drafts -> approval -> execution)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF,TECHNICIAN")]
public class WorkOrderController : ControllerBase
{
    private readonly IWorkOrderService _workOrderService;

    public WorkOrderController(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
    }

    [HttpPost]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _workOrderService.CreateAsync(dto, ct);
        if (result.IsSuccess && result.Data != null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkOrderDto dto, CancellationToken ct)
    {
        var result = await _workOrderService.UpdateAsync(id, dto, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}/submit")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> Submit(Guid id, [FromBody] SubmitWorkOrderDto dto, CancellationToken ct)
    {
        dto.WorkOrderId = id;
        var result = await _workOrderService.SubmitAsync(dto, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = "ADMIN,STAFF")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveWorkOrderDto dto, CancellationToken ct)
    {
        dto.WorkOrderId = id;
        var result = await _workOrderService.ApproveAsync(dto, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = "ADMIN,STAFF")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectWorkOrderDto dto, CancellationToken ct)
    {
        dto.WorkOrderId = id;
        var result = await _workOrderService.RejectAsync(dto, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}/revise")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> Revise(Guid id, [FromBody] ReviseWorkOrderDto dto, CancellationToken ct)
    {
        dto.WorkOrderId = id;
        var result = await _workOrderService.ReviseAsync(dto, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}/start")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> Start(Guid id, [FromBody] StartWorkDto dto, CancellationToken ct)
    {
        dto.WorkOrderId = id;
        var result = await _workOrderService.StartAsync(dto, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{id:guid}/complete")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteWorkDto dto, CancellationToken ct)
    {
        dto.WorkOrderId = id;
        var result = await _workOrderService.CompleteAsync(dto, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _workOrderService.GetByIdAsync(id, ct);
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);
        return NotFound(result.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetRange([FromQuery] Guid? centerId, [FromQuery] DateOnly? date, [FromQuery] string? status, [FromQuery] Guid? technicianId, CancellationToken ct)
    {
        var result = await _workOrderService.GetRangeAsync(centerId, date, status, technicianId, ct);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }
}

