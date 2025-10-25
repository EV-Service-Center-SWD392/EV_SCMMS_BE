using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Assignmentthaontt entity operations
/// </summary>
public interface IAssignmentRepository : IGenericRepository<Assignmentthaontt>
{
    /// <summary>
    /// Counts active assignments for a technician that overlap a given UTC time range.
    /// Overlap rule: (AStart < BEnd) && (BStart < AEnd)
    /// </summary>
    Task<int> CountAssignmentsByTechnicianAndRangeAsync(
        Guid technicianId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct = default);

    /// <summary>
    /// Checks if there exists any active assignment for the technician that overlaps the given time range.
    /// Excludes a specific assignment id when provided.
    /// </summary>
    Task<bool> ExistsOverlapAsync(
        Guid technicianId,
        DateTime startUtc,
        DateTime endUtc,
        Guid? excludeAssignmentId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieves assignments filtered by optional center, date, and status.
    /// </summary>
    Task<List<Assignmentthaontt>> GetRangeAsync(
        Guid? centerId,
        DateOnly? date,
        string? status,
        CancellationToken ct = default);
}
