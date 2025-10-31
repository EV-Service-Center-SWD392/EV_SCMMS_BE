using EV_SCMMS.Core.Application.DTOs.Center;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Center service interface for business logic operations
/// </summary>
public interface ICenterService
{
    /// <summary>
    /// Get all centers with pagination
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paged result of centers</returns>
    Task<IServiceResult<List<CenterDto>>> GetAllAsync();

    /// <summary>
    /// Get center by ID
    /// </summary>
    /// <param name="id">Center ID</param>
    /// <returns>Center DTO</returns>
    Task<IServiceResult<CenterDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Create new center
    /// </summary>
    /// <param name="createDto">Center creation DTO</param>
    /// <returns>Created center DTO</returns>
    Task<IServiceResult<CenterDto>> CreateAsync(CreateCenterDto createDto);

    /// <summary>
    /// Update existing center
    /// </summary>
    /// <param name="id">Center ID</param>
    /// <param name="updateDto">Center update DTO</param>
    /// <returns>Updated center DTO</returns>
    Task<IServiceResult<CenterDto>> UpdateAsync(Guid id, UpdateCenterDto updateDto);

    /// <summary>
    /// Soft delete center
    /// </summary>
    /// <param name="id">Center ID</param>
    /// <returns>Result of deletion</returns>
    Task<IServiceResult<bool>> DeleteAsync(Guid id);

    /// <summary>
    /// Get active centers only
    /// </summary>
    /// <returns>List of active centers</returns>
    Task<IServiceResult<List<CenterDto>>> GetActiveCentersAsync();

    /// <summary>
    /// Search centers by name
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paged result of matching centers</returns>
    Task<IServiceResult<List<CenterDto>>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10);
}