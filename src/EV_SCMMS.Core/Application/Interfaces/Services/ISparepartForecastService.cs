using EV_SCMMS.Core.Application.DTOs.SparepartForecast;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// SparepartForecast service interface for business logic operations
/// </summary>
public interface ISparepartForecastService
{
    /// <summary>
    /// Get all sparepart forecasts
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paged result of sparepart forecasts</returns>
    Task<IServiceResult<List<SparepartForecastDto>>> GetAllAsync();

    /// <summary>
    /// Get sparepart forecast by ID
    /// </summary>
    /// <param name="id">Forecast ID</param>
    /// <returns>SparepartForecast DTO</returns>
    Task<IServiceResult<SparepartForecastDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Create new sparepart forecast
    /// </summary>
    /// <param name="createDto">SparepartForecast creation DTO</param>
    /// <returns>Created sparepart forecast DTO</returns>
    Task<IServiceResult<SparepartForecastDto>> CreateAsync(CreateSparepartForecastDto createDto);

    /// <summary>
    /// Update existing sparepart forecast
    /// </summary>
    /// <param name="id">Forecast ID</param>
    /// <param name="updateDto">SparepartForecast update DTO</param>
    /// <returns>Updated sparepart forecast DTO</returns>
    Task<IServiceResult<SparepartForecastDto>> UpdateAsync(Guid id, UpdateSparepartForecastDto updateDto);

    /// <summary>
    /// Soft delete sparepart forecast
    /// </summary>
    /// <param name="id">Forecast ID</param>
    /// <returns>Result of deletion</returns>
    Task<IServiceResult<bool>> DeleteAsync(Guid id);

    /// <summary>
    /// Get forecasts by sparepart ID
    /// </summary>
    /// <param name="sparepartId">Sparepart ID</param>
    /// <returns>List of forecasts for the sparepart</returns>
    Task<IServiceResult<List<SparepartForecastDto>>> GetBySparepartIdAsync(Guid sparepartId);

    /// <summary>
    /// Get forecasts by center ID
    /// </summary>
    /// <param name="centerId">Center ID</param>
    /// <returns>List of forecasts for the center</returns>
    Task<IServiceResult<List<SparepartForecastDto>>> GetByCenterIdAsync(Guid centerId);

    /// <summary>
    /// Get forecasts with low reorder points
    /// </summary>
    /// <returns>List of forecasts with low reorder points</returns>
    Task<IServiceResult<List<SparepartForecastDto>>> GetLowReorderPointForecastsAsync();

    /// <summary>
    /// Approve forecast
    /// </summary>
    /// <param name="id">Forecast ID</param>
    /// <param name="approvedBy">Approver name</param>
    /// <returns>Updated forecast DTO</returns>
    Task<IServiceResult<SparepartForecastDto>> ApproveForecastAsync(Guid id, string approvedBy);
}