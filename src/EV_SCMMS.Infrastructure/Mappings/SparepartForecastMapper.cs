using EV_SCMMS.Core.Application.DTOs.SparepartForecast;
using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Infrastructure.Mappings;

/// <summary>
/// Extension methods for SparepartForecast entity mapping
/// </summary>
public static class SparepartForecastMapperExtensions
{
    /// <summary>
    /// Map SparepartforecastTuht entity to SparepartForecastDto
    /// </summary>
    /// <param name="forecast">SparepartforecastTuht entity</param>
    /// <returns>SparepartForecastDto</returns>
    public static SparepartForecastDto ToDto(this SparepartforecastTuht forecast)
    {
        if (forecast == null) return null;

        return new SparepartForecastDto
        {
            ForecastId = forecast.Forecastid,
            SparepartId = forecast.Sparepartid,
            CenterId = forecast.Centerid,
            PredictedUsage = forecast.Predictedusage,
            SafetyStock = forecast.Safetystock,
            ReorderPoint = forecast.Reorderpoint,
            ForecastedBy = forecast.Forecastedby,
            ForecastConfidence = forecast.Forecastconfidence,
            ForecastDate = forecast.Forecastdate,
            ApprovedBy = forecast.Approvedby,
            ApprovedDate = forecast.Approveddate,
            Status = forecast.Status,
            CreatedAt = forecast.Createdat,
            UpdatedAt = forecast.Updatedat,
            SparepartName = forecast.Sparepart?.Name,
            CenterName = forecast.Center?.Name
        };
    }

    /// <summary>
    /// Map list of SparepartforecastTuht entities to list of SparepartForecastDto
    /// </summary>
    /// <param name="forecasts">List of SparepartforecastTuht entities</param>
    /// <returns>List of SparepartForecastDto</returns>
    public static List<SparepartForecastDto> ToDto(this IEnumerable<SparepartforecastTuht> forecasts)
    {
        if (forecasts == null) return new List<SparepartForecastDto>();
        
        return forecasts.Select(f => f.ToDto()).Where(dto => dto != null).ToList();
    }

    /// <summary>
    /// Map CreateSparepartForecastDto to SparepartforecastTuht entity
    /// </summary>
    /// <param name="createDto">CreateSparepartForecastDto</param>
    /// <returns>SparepartforecastTuht entity</returns>
    public static SparepartforecastTuht ToEntity(this CreateSparepartForecastDto createDto)
    {
        if (createDto == null) return null;

        return new SparepartforecastTuht
        {
            Forecastid = Guid.NewGuid(),
            Sparepartid = createDto.SparepartId,
            Centerid = createDto.CenterId,
            Predictedusage = createDto.PredictedUsage,
            Safetystock = createDto.SafetyStock,
            Reorderpoint = createDto.ReorderPoint,
            Forecastedby = createDto.ForecastedBy,
            Forecastconfidence = createDto.ForecastConfidence,
            Forecastdate = createDto.ForecastDate,
            Approvedby = null, // Will be set during approval
            Approveddate = null, // Will be set during approval
            Status = createDto.Status ?? "Pending",
            Isactive = true,
            Createdat = DateTime.UtcNow,
            Updatedat = null
        };
    }

    /// <summary>
    /// Update existing SparepartforecastTuht entity with UpdateSparepartForecastDto data
    /// </summary>
    /// <param name="entity">Existing SparepartforecastTuht entity</param>
    /// <param name="updateDto">UpdateSparepartForecastDto</param>
    public static void UpdateFromDto(this SparepartforecastTuht entity, UpdateSparepartForecastDto updateDto)
    {
        if (entity == null || updateDto == null) return;

        entity.Sparepartid = updateDto.SparepartId;
        entity.Centerid = updateDto.CenterId;
        entity.Predictedusage = updateDto.PredictedUsage;
        entity.Safetystock = updateDto.SafetyStock;
        entity.Reorderpoint = updateDto.ReorderPoint;
        entity.Forecastedby = updateDto.ForecastedBy;
        entity.Forecastconfidence = updateDto.ForecastConfidence;
        entity.Forecastdate = updateDto.ForecastDate;
        entity.Status = updateDto.Status;
        entity.Updatedat = DateTime.UtcNow;
    }
}