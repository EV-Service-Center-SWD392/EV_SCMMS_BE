using System.Security.Claims;
using EV_SCMMS.Core.Application.DTOs.ChecklistItems;
using EV_SCMMS.Core.Application.DTOs.Common;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// CRUD for ThaoNTT checklist catalog items
/// Examples:
/// - GET /api/checklist/items?q=battery&status=ACTIVE&page=1&pageSize=20&sort=createdAt&order=desc
/// - POST /api/checklist/items { "code":"BAT_SOC","name":"Battery SOC","type":2,"unit":"%" }
/// - PUT /api/checklist/items/{id} { "name":"Battery State of Charge","unit":"%" }
/// - DELETE /api/checklist/items/{id}
/// - PATCH /api/checklist/items/{id}/activate { "isActive": false }
/// </summary>
[ApiController]
[Route("api/checklist/items")]
[Authorize(Roles = "ADMIN,STAFF,TECHNICIAN")]
public class ChecklistItemsController : ControllerBase
{
    private readonly IChecklistItemService _service;

    public ChecklistItemsController(IChecklistItemService service)
    {
        _service = service;
    }

    // Note: List endpoint is handled in ChecklistController to preserve legacy route without ambiguity.
    // This controller provides detail + write operations.

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound("Item not found");
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,STAFF")]
    public async Task<IActionResult> Create([FromBody] CreateChecklistItemDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var actorId = GetActorId();
        try
        {
            var created = await _service.CreateAsync(dto, actorId, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN,STAFF")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChecklistItemDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var actorId = GetActorId();
        try
        {
            var updated = await _service.UpdateAsync(id, dto, actorId, ct);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Item not found");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN,STAFF")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var actorId = GetActorId();
        var ok = await _service.SoftDeleteAsync(id, actorId, ct);
        if (!ok) return NotFound("Item not found");
        return Ok(new { success = true });
    }

    public class SetActiveRequest { public bool IsActive { get; set; } }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "ADMIN,STAFF")]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetActiveRequest body, CancellationToken ct)
    {
        var actorId = GetActorId();
        try
        {
            var dto = await _service.SetActiveAsync(id, body.IsActive, actorId, ct);
            return Ok(dto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Item not found");
        }
    }

    private Guid GetActorId()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(id, out var gid) ? gid : Guid.Empty;
    }
}

