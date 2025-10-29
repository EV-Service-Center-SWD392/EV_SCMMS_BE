using EV_SCMMS.Core.Application.DTOs.SparepartUsageHistory;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// SparepartUsageHistory service interface for business logic operations
/// </summary>
public interface ISparepartUsageHistoryService
{
    /// <summary>
    /// Get all usage histories with pagination
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paged result of usage histories</returns>
    Task<IServiceResult<List<SparepartUsageHistoryDto>>> GetAllAsync();

    /// <summary>
    /// Get usage history by ID
    /// </summary>
    /// <param name="id">Usage ID</param>
    /// <returns>SparepartUsageHistory DTO</returns>
    Task<IServiceResult<SparepartUsageHistoryDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Create new usage history
    /// </summary>
    /// <param name="createDto">SparepartUsageHistory creation DTO</param>
    /// <returns>Created usage history DTO</returns>
    Task<IServiceResult<SparepartUsageHistoryDto>> CreateAsync(CreateSparepartUsageHistoryDto createDto);

    /// <summary>
    /// Update existing usage history
    /// </summary>
    /// <param name="id">Usage ID</param>
    /// <param name="updateDto">SparepartUsageHistory update DTO</param>
    /// <returns>Updated usage history DTO</returns>
    Task<IServiceResult<SparepartUsageHistoryDto>> UpdateAsync(Guid id, UpdateSparepartUsageHistoryDto updateDto);

    /// <summary>
    /// Soft delete usage history
    /// </summary>
    /// <param name="id">Usage ID</param>
    /// <returns>Result of deletion</returns>
    Task<IServiceResult<bool>> DeleteAsync(Guid id);

    /// <summary>
    /// Get usage histories by sparepart ID
    /// </summary>
    /// <param name="sparepartId">Sparepart ID</param>
    /// <returns>List of usage histories for the sparepart</returns>
    Task<IServiceResult<List<SparepartUsageHistoryDto>>> GetBySparepartIdAsync(Guid sparepartId);

    /// <summary>
    /// Get usage histories by center ID
    /// </summary>
    /// <param name="centerId">Center ID</param>
    /// <returns>List of usage histories for the center</returns>
    Task<IServiceResult<List<SparepartUsageHistoryDto>>> GetByCenterIdAsync(Guid centerId);

    /// <summary>
    /// Get usage histories by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of usage histories in the date range</returns>
    Task<IServiceResult<List<SparepartUsageHistoryDto>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get usage histories by vehicle ID
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <returns>List of usage histories for the vehicle</returns>
    Task<IServiceResult<List<SparepartUsageHistoryDto>>> GetByVehicleIdAsync(string vehicleId);

    /// <summary>
    /// Get usage statistics for a sparepart
    /// </summary>
    /// <param name="sparepartId">Sparepart ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Usage statistics</returns>
    Task<IServiceResult<object>> GetUsageStatisticsAsync(Guid sparepartId, DateTime startDate, DateTime endDate);
}