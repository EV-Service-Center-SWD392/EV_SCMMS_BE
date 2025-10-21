using EV_SCMMS.Core.Application.DTOs.SparepartReplenishmentRequest;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for SparepartReplenishmentRequest entity mapping
/// </summary>
public static class SparepartReplenishmentRequestMapperExtensions
{
    /// <summary>
    /// Map Sparepartreplenishmentrequest entity to SparepartReplenishmentRequestDto
    /// </summary>
    /// <param name="request">Sparepartreplenishmentrequest entity</param>
    /// <returns>SparepartReplenishmentRequestDto</returns>
    public static SparepartReplenishmentRequestDto ToDto(this Sparepartreplenishmentrequest request)
    {
        if (request == null) return null;

        return new SparepartReplenishmentRequestDto
        {
            Id = request.Requestid,
            SparepartId = request.Sparepartid,
            CenterId = request.Centerid,
            RequestedQuantity = request.Suggestedquantity ?? 0,
            RequestedBy = "", // Not available in model
            ApprovedBy = request.Approvedby?.ToString(),
            SupplierId = null, // Not available in model
            EstimatedCost = null, // Not available in model
            Priority = "", // Not available in model
            Status = request.Status ?? "Pending",
            Notes = request.Notes,
            RequestDate = request.Createdat ?? DateTime.UtcNow,
            ApprovalDate = request.Approvedat,
            ExpectedDeliveryDate = null, // Not available in model
            ActualDeliveryDate = null, // Not available in model
            CreatedAt = request.Createdat ?? DateTime.UtcNow,
            UpdatedAt = request.Updatedat,
            IsDeleted = !(request.Isactive ?? true),
            Sparepart = null, // Will be populated separately if needed
            Center = null // Will be populated separately if needed
        };
    }

    /// <summary>
    /// Map list of Sparepartreplenishmentrequest entities to list of SparepartReplenishmentRequestDto
    /// </summary>
    /// <param name="requests">List of Sparepartreplenishmentrequest entities</param>
    /// <returns>List of SparepartReplenishmentRequestDto</returns>
    public static List<SparepartReplenishmentRequestDto> ToDto(this IEnumerable<Sparepartreplenishmentrequest> requests)
    {
        if (requests == null) return new List<SparepartReplenishmentRequestDto>();
        
        return requests.Select(r => r.ToDto()).Where(dto => dto != null).ToList();
    }

    /// <summary>
    /// Map CreateSparepartReplenishmentRequestDto to Sparepartreplenishmentrequest entity
    /// </summary>
    /// <param name="createDto">CreateSparepartReplenishmentRequestDto</param>
    /// <returns>Sparepartreplenishmentrequest entity</returns>
    public static Sparepartreplenishmentrequest ToEntity(this CreateSparepartReplenishmentRequestDto createDto)
    {
        if (createDto == null) return null;

        return new Sparepartreplenishmentrequest
        {
            Requestid = Guid.NewGuid(),
            Centerid = createDto.CenterId,
            Sparepartid = createDto.SparepartId,
            Forecastid = null, // Not in CreateDto
            Suggestedquantity = createDto.RequestedQuantity,
            Status = createDto.Status ?? "Pending",
            Approvedby = null, // Will be set during approval
            Approvedat = null, // Will be set during approval
            Notes = createDto.Notes,
            Isactive = true,
            Createdat = DateTime.UtcNow,
            Updatedat = null
        };
    }

    /// <summary>
    /// Update existing Sparepartreplenishmentrequest entity with UpdateSparepartReplenishmentRequestDto data
    /// </summary>
    /// <param name="entity">Existing Sparepartreplenishmentrequest entity</param>
    /// <param name="updateDto">UpdateSparepartReplenishmentRequestDto</param>
    public static void UpdateFromDto(this Sparepartreplenishmentrequest entity, UpdateSparepartReplenishmentRequestDto updateDto)
    {
        if (entity == null || updateDto == null) return;

        entity.Centerid = updateDto.CenterId;
        entity.Sparepartid = updateDto.SparepartId;
        entity.Suggestedquantity = updateDto.RequestedQuantity;
        entity.Status = updateDto.Status;
        entity.Notes = updateDto.Notes;
        entity.Updatedat = DateTime.UtcNow;
    }
}