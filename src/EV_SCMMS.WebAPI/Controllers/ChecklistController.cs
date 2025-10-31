using EV_SCMMS.Core.Application.DTOs.ServiceIntake;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// API endpoints for EV service checklist items and responses
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF,TECHNICIAN")]
public class ChecklistController : ControllerBase
{
    private readonly IChecklistService _checklistService;

    public ChecklistController(IChecklistService checklistService)
    {
        _checklistService = checklistService;
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetItems([FromQuery] string? q, [FromQuery] string? status, [FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? sort, [FromQuery] string? order, CancellationToken ct)
    {
        // If advanced query params are provided, serve paged admin-style listing via new service
        if (!string.IsNullOrWhiteSpace(q) || !string.IsNullOrWhiteSpace(status) || page.HasValue || pageSize.HasValue || !string.IsNullOrWhiteSpace(sort) || !string.IsNullOrWhiteSpace(order))
        {
            var itemService = HttpContext.RequestServices.GetService(typeof(IChecklistItemService)) as IChecklistItemService;
            if (itemService == null)
            {
                return BadRequest("Checklist item service not available");
            }

            var paged = await itemService.GetAsync(q, status ?? "ACTIVE", page ?? 1, pageSize ?? 20, sort ?? "createdAt", order ?? "desc", ct);
            return Ok(paged);
        }

        var result = await _checklistService.GetItemsAsync(ct);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpGet("{intakeId:guid}/responses")]
    public async Task<IActionResult> GetResponses(Guid intakeId, CancellationToken ct)
    {
        var result = await _checklistService.GetResponsesAsync(intakeId, ct);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.Message);
    }

    [HttpPut("{intakeId:guid}/responses")]
    public async Task<IActionResult> UpsertResponses(Guid intakeId, [FromBody] UpsertChecklistResponsesDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        dto.IntakeId = intakeId;
        var result = await _checklistService.UpsertResponsesAsync(dto, ct);
        if (result.IsSuccess) return Ok(new { success = true, message = result.Message ?? "Responses saved" });
        return BadRequest(result.Message);
    }
}
