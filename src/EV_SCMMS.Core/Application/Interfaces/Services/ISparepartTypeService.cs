using EV_SCMMS.Core.Application.DTOs.SparepartType;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;


public interface ISparepartTypeService
{
    /// <summary>
    /// Get all sparepart types with pagination
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paged result of sparepart types</returns>
    Task<IServiceResult<List<SparepartTypeDto>>> GetAllAsync();

    /// <summary>
    /// Get sparepart type by ID
    /// </summary>
    /// <param name="id">Type ID</param>
    /// <returns>SparepartType DTO</returns>
    Task<IServiceResult<SparepartTypeDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Create new sparepart type
    /// </summary>
    /// <param name="createDto">SparepartType creation DTO</param>
    /// <returns>Created sparepart type DTO</returns>
    Task<IServiceResult<SparepartTypeDto>> CreateAsync(CreateSparepartTypeDto createDto);

    /// <summary>
    /// Update existing sparepart type
    /// </summary>
    /// <param name="id">Type ID</param>
    /// <param name="updateDto">SparepartType update DTO</param>
    /// <returns>Updated sparepart type DTO</returns>
    Task<IServiceResult<SparepartTypeDto>> UpdateAsync(Guid id, UpdateSparepartTypeDto updateDto);

    /// <summary>
    /// Soft delete sparepart type
    /// </summary>
    /// <param name="id">Type ID</param>
    /// <returns>Result of deletion</returns>
    Task<IServiceResult<bool>> DeleteAsync(Guid id);

    /// <summary>
    /// Get all active sparepart types
    /// </summary>
    /// <returns>List of active sparepart types</returns>
    Task<IServiceResult<List<SparepartTypeDto>>> GetActiveTypesAsync();

    /// <summary>
    /// Search sparepart types by name
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paged result of matching sparepart types</returns>
    Task<IServiceResult<List<SparepartTypeDto>>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10);
}