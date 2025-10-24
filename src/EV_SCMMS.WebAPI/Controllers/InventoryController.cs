using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.Inventory;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Controller for managing inventory operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Get all inventories with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of inventories</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllInventories()
    {
        var result = await _inventoryService.GetAllAsync();

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get inventory by ID
    /// </summary>
    /// <param name="id">Inventory ID</param>
    /// <returns>Inventory details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInventoryById(Guid id)
    {
        var result = await _inventoryService.GetByIdAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return NotFound(result.Message);
    }

    /// <summary>
    /// Create new inventory
    /// </summary>
    /// <param name="createDto">Inventory creation data</param>
    /// <returns>Created inventory</returns>
    [HttpPost]
    public async Task<IActionResult> CreateInventory([FromBody] CreateInventoryDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _inventoryService.CreateAsync(createDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetInventoryById), new { id = result.Data.InventoryId }, result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Update existing inventory
    /// </summary>
    /// <param name="id">Inventory ID</param>
    /// <param name="updateDto">Inventory update data</param>
    /// <returns>Updated inventory</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInventory(Guid id, [FromBody] UpdateInventoryDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _inventoryService.UpdateAsync(id, updateDto);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Delete inventory (soft delete)
    /// </summary>
    /// <param name="id">Inventory ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventory(Guid id)
    {
        var result = await _inventoryService.DeleteAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(new { message = "Inventory deleted successfully" });
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get inventories by center ID
    /// </summary>
    /// <param name="centerId">Center ID</param>
    /// <returns>List of inventories for the center</returns>
    [HttpGet("center/{centerId}")]
    public async Task<IActionResult> GetInventoriesByCenter(Guid centerId)
    {
        var result = await _inventoryService.GetByCenterIdAsync(centerId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    // TODO: Implement these methods in IInventoryService
    /*
    /// <summary>
    /// Get low stock inventories
    /// </summary>
    /// <returns>List of inventories with low stock</returns>
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockInventories()
    {
        var result = await _inventoryService.GetLowStockAsync();
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get inventories by status
    /// </summary>
    /// <param name="status">Inventory status</param>
    /// <returns>List of inventories with the specified status</returns>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetInventoriesByStatus(string status)
    {
        var result = await _inventoryService.GetByStatusAsync(status);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get active inventories
    /// </summary>
    /// <returns>List of active inventories</returns>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveInventories()
    {
        var result = await _inventoryService.GetActiveInventoriesAsync();
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }
    */
}