using EV_SCMMS.Core.Application.DTOs.SparepartUsageHistory;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for SparepartUsageHistory entity mapping
/// </summary>
public static class SparepartUsageHistoryMapperExtensions
{
    /// <summary>
    /// Map SparepartusagehistoryTuht entity to SparepartUsageHistoryDto
    /// </summary>
    /// <param name="usageHistory">SparepartusagehistoryTuht entity</param>
    /// <returns>SparepartUsageHistoryDto</returns>
    public static SparepartUsageHistoryDto ToDto(this SparepartusagehistoryTuht usageHistory)
    {
        if (usageHistory == null) return null;

        return new SparepartUsageHistoryDto
        {
            Id = usageHistory.Usageid,
            SparepartId = usageHistory.Sparepartid,
            CenterId = usageHistory.Centerid,
            QuantityUsed = usageHistory.Quantityused,
            UsedBy = "", // Not available in model
            VehicleId = null, // Not available in model
            WorkOrderId = null, // Not available in model
            MaintenanceType = null, // Not available in model
            Purpose = null, // Not available in model
            UsageDate = usageHistory.Useddate ?? DateTime.UtcNow,
            Notes = null, // Not available in model
            CreatedAt = usageHistory.Createdat ?? DateTime.UtcNow,
            UpdatedAt = usageHistory.Updatedat,
            IsDeleted = !(usageHistory.Isactive ?? true),
            Sparepart = null, // Will be populated separately if needed
            Center = null // Will be populated separately if needed
        };
    }

    /// <summary>
    /// Map list of SparepartusagehistoryTuht entities to list of SparepartUsageHistoryDto
    /// </summary>
    /// <param name="usageHistories">List of SparepartusagehistoryTuht entities</param>
    /// <returns>List of SparepartUsageHistoryDto</returns>
    public static List<SparepartUsageHistoryDto> ToDto(this IEnumerable<SparepartusagehistoryTuht> usageHistories)
    {
        if (usageHistories == null) return new List<SparepartUsageHistoryDto>();
        
        return usageHistories.Select(uh => uh.ToDto()).Where(dto => dto != null).ToList();
    }

    /// <summary>
    /// Map CreateSparepartUsageHistoryDto to SparepartusagehistoryTuht entity
    /// </summary>
    /// <param name="createDto">CreateSparepartUsageHistoryDto</param>
    /// <returns>SparepartusagehistoryTuht entity</returns>
    public static SparepartusagehistoryTuht ToEntity(this CreateSparepartUsageHistoryDto createDto)
    {
        if (createDto == null) return null;

        return new SparepartusagehistoryTuht
        {
            Usageid = Guid.NewGuid(),
            Sparepartid = createDto.SparepartId,
            Centerid = createDto.CenterId,
            Quantityused = createDto.QuantityUsed,
            Useddate = createDto.UsageDate,
            Status = "Active",
            Isactive = true,
            Createdat = DateTime.UtcNow,
            Updatedat = null
        };
    }

    /// <summary>
    /// Update existing SparepartusagehistoryTuht entity with UpdateSparepartUsageHistoryDto data
    /// </summary>
    /// <param name="entity">Existing SparepartusagehistoryTuht entity</param>
    /// <param name="updateDto">UpdateSparepartUsageHistoryDto</param>
    public static void UpdateFromDto(this SparepartusagehistoryTuht entity, UpdateSparepartUsageHistoryDto updateDto)
    {
        if (entity == null || updateDto == null) return;

        entity.Sparepartid = updateDto.SparepartId;
        entity.Centerid = updateDto.CenterId;
        entity.Quantityused = updateDto.QuantityUsed;
        entity.Useddate = updateDto.UsageDate;
        entity.Updatedat = DateTime.UtcNow;
    }
}