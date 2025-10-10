using Microsoft.AspNetCore.Mvc;
using EV_SCMMS.Core.Application.DTOs.Sparepart;
using EV_SCMMS.Core.Application.Interfaces.Services;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Controller for managing sparepart operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SparepartController : ControllerBase
{
    private readonly ISparepartService _sparepartService;

    public SparepartController(ISparepartService sparepartService)
    {
        _sparepartService = sparepartService;
    }

    /// <summary>
    /// Get all spareparts with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of spareparts</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllSpareparts()
    {
        var result = await _sparepartService.GetAllAsync();

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get sparepart by ID
    /// </summary>
    /// <param name="id">Sparepart ID</param>
    /// <returns>Sparepart details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSparepartById(Guid id)
    {
        var result = await _sparepartService.GetByIdAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return NotFound(result.Message);
    }

    /// <summary>
    /// Create new sparepart
    /// </summary>
    /// <param name="createDto">Sparepart creation data</param>
    /// <returns>Created sparepart</returns>
    [HttpPost]
    public async Task<IActionResult> CreateSparepart([FromBody] CreateSparepartDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _sparepartService.CreateAsync(createDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetSparepartById), new { id = result.Data.SparepartId }, result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Update existing sparepart
    /// </summary>
    /// <param name="id">Sparepart ID</param>
    /// <param name="updateDto">Sparepart update data</param>
    /// <returns>Updated sparepart</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSparepart(Guid id, [FromBody] UpdateSparepartDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _sparepartService.UpdateAsync(id, updateDto);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Delete sparepart (soft delete)
    /// </summary>
    /// <param name="id">Sparepart ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSparepart(Guid id)
    {
        var result = await _sparepartService.DeleteAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(new { message = "Sparepart deleted successfully" });
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get spareparts by type ID
    /// </summary>
    /// <param name="typeId">Sparepart type ID</param>
    /// <returns>List of spareparts for the type</returns>
    [HttpGet("type/{typeId}")]
    public async Task<IActionResult> GetSparepartsByType(Guid typeId)
    {
        var result = await _sparepartService.GetByTypeIdAsync(typeId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get spareparts by vehicle model ID
    /// </summary>
    /// <param name="vehicleModelId">Vehicle model ID</param>
    /// <returns>List of spareparts for the vehicle model</returns>
    [HttpGet("vehicle-model/{vehicleModelId}")]
    public async Task<IActionResult> GetSparepartsByVehicleModel(int vehicleModelId)
    {
        var result = await _sparepartService.GetByVehicleModelIdAsync(Guid.Parse(vehicleModelId.ToString()));
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get spareparts by manufacturer
    /// </summary>
    /// <param name="manufacturer">Manufacturer name</param>
    /// <returns>List of spareparts from the manufacturer</returns>
    [HttpGet("manufacturer/{manufacturer}")]
    public async Task<IActionResult> GetSparepartsByManufacturer(string manufacturer)
    {
        var result = await _sparepartService.GetByManufacturerAsync(manufacturer);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }

    /// <summary>
    /// Get spareparts by price range - TODO: Implement in service
    /// </summary>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <returns>List of spareparts in the price range</returns>
    [HttpGet("price-range")]
    public IActionResult GetSparepartsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        // var result = await _sparepartService.GetByPriceRangeAsync(minPrice, maxPrice);
        return BadRequest("Method not implemented in service");
    }

    /// <summary>
    /// Get active spareparts - TODO: Implement in service
    /// </summary>
    /// <returns>List of active spareparts</returns>
    [HttpGet("active")]
    public IActionResult GetActiveSpareparts()
    {
        // var result = await _sparepartService.GetActiveSparepartsAsync();
        return BadRequest("Method not implemented in service");
    }

    /// <summary>
    /// Search spareparts by name
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>List of matching spareparts</returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchSpareparts([FromQuery] string searchTerm)
    {
        var result = await _sparepartService.SearchAsync(searchTerm);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.Message);
    }
}