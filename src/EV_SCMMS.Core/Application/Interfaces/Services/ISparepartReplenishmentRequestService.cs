using EV_SCMMS.Core.Application.DTOs.SparepartReplenishmentRequest;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// SparepartReplenishmentRequest service interface for business logic operations
/// </summary>
public interface ISparepartReplenishmentRequestService
{
    /// <summary>
    /// Get all replenishment requests with pagination
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paged result of replenishment requests</returns>
    Task<IServiceResult<List<SparepartReplenishmentRequestDto>>> GetAllAsync();

    /// <summary>
    /// Get replenishment request by ID
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <returns>SparepartReplenishmentRequest DTO</returns>
    Task<IServiceResult<SparepartReplenishmentRequestDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Create new replenishment request
    /// </summary>
    /// <param name="createDto">SparepartReplenishmentRequest creation DTO</param>
    /// <returns>Created replenishment request DTO</returns>
    Task<IServiceResult<SparepartReplenishmentRequestDto>> CreateAsync(CreateSparepartReplenishmentRequestDto createDto);

    /// <summary>
    /// Update existing replenishment request
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="updateDto">SparepartReplenishmentRequest update DTO</param>
    /// <returns>Updated replenishment request DTO</returns>
    Task<IServiceResult<SparepartReplenishmentRequestDto>> UpdateAsync(Guid id, UpdateSparepartReplenishmentRequestDto updateDto);

    /// <summary>
    /// Soft delete replenishment request
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <returns>Result of deletion</returns>
    Task<IServiceResult<bool>> DeleteAsync(Guid id);

    /// <summary>
    /// Get requests by sparepart ID
    /// </summary>
    /// <param name="sparepartId">Sparepart ID</param>
    /// <returns>List of requests for the sparepart</returns>
    Task<IServiceResult<List<SparepartReplenishmentRequestDto>>> GetBySparepartIdAsync(Guid sparepartId);

    /// <summary>
    /// Get requests by center ID
    /// </summary>
    /// <param name="centerId">Center ID</param>
    /// <returns>List of requests for the center</returns>
    Task<IServiceResult<List<SparepartReplenishmentRequestDto>>> GetByCenterIdAsync(Guid centerId);

    /// <summary>
    /// Get requests by status
    /// </summary>
    /// <param name="status">Request status</param>
    /// <returns>List of requests with the specified status</returns>
    Task<IServiceResult<List<SparepartReplenishmentRequestDto>>> GetByStatusAsync(string status);

    /// <summary>
    /// Approve replenishment request
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="approveDto">Approval details including approver ID and notes</param>
    /// <returns>Updated request DTO</returns>
    Task<IServiceResult<SparepartReplenishmentRequestDto>> ApproveRequestAsync(Guid id, ApproveRequestDto approveDto);

    /// <summary>
    /// Reject replenishment request
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="rejectDto">Rejection details including rejector ID, reason and notes</param>
    /// <returns>Updated request DTO</returns>
    Task<IServiceResult<SparepartReplenishmentRequestDto>> RejectRequestAsync(Guid id, RejectRequestDto rejectDto);

    /// <summary>
    /// Get pending requests
    /// </summary>
    /// <returns>List of pending requests</returns>
    Task<IServiceResult<List<SparepartReplenishmentRequestDto>>> GetPendingRequestsAsync();
}