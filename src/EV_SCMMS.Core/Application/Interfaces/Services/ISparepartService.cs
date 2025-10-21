using EV_SCMMS.Core.Application.DTOs.Sparepart;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Sparepart service interface for business logic operations
/// </summary>
public interface ISparepartService
{
    /// <summary>
    /// Get all spareparts
    /// </summary>
    /// <returns>List of all spareparts</returns>
    Task<IServiceResult<List<SparepartDto>>> GetAllAsync();

    /// <summary>
    /// Get sparepart by ID
    /// </summary>
    /// <param name="id">Sparepart ID</param>
    /// <returns>Sparepart DTO</returns>
    Task<IServiceResult<SparepartDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Create new sparepart
    /// </summary>
    /// <param name="createDto">Sparepart creation DTO</param>
    /// <returns>Created sparepart DTO</returns>
    Task<IServiceResult<SparepartDto>> CreateAsync(CreateSparepartDto createDto);

    /// <summary>
    /// Update existing sparepart
    /// </summary>
    /// <param name="id">Sparepart ID</param>
    /// <param name="updateDto">Sparepart update DTO</param>
    /// <returns>Updated sparepart DTO</returns>
    Task<IServiceResult<SparepartDto>> UpdateAsync(Guid id, UpdateSparepartDto updateDto);

    /// <summary>
    /// Soft delete sparepart
    /// </summary>
    /// <param name="id">Sparepart ID</param>
    /// <returns>Result of deletion</returns>
    Task<IServiceResult<bool>> DeleteAsync(Guid id);

    /// <summary>
    /// Get spareparts by type ID
    /// </summary>
    /// <param name="typeId">Type ID</param>
    /// <returns>List of spareparts of the specified type</returns>
    Task<IServiceResult<List<SparepartDto>>> GetByTypeIdAsync(Guid typeId);

    /// <summary>
    /// Get spareparts by vehicle model ID
    /// </summary>
    /// <param name="vehicleModelId">Vehicle model ID</param>
    /// <returns>List of spareparts for the vehicle model</returns>
    Task<IServiceResult<List<SparepartDto>>> GetByVehicleModelIdAsync(Guid vehicleModelId);

    /// <summary>
    /// Search spareparts by part number or name
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>List of matching spareparts</returns>
    Task<IServiceResult<List<SparepartDto>>> SearchAsync(string searchTerm);

    /// <summary>
    /// Get spareparts by manufacturer
    /// </summary>
    /// <param name="manufacturer">Manufacturer name</param>
    /// <returns>List of spareparts from the manufacturer</returns>
    Task<IServiceResult<List<SparepartDto>>> GetByManufacturerAsync(string manufacturer);
}