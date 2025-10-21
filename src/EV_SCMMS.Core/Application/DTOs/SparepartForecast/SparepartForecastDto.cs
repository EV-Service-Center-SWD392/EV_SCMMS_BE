using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.SparepartForecast;

/// <summary>
/// SparepartForecast DTO for response
/// </summary>
public class SparepartForecastDto
{
    public Guid ForecastId { get; set; }

    public Guid SparepartId { get; set; }

    public Guid CenterId { get; set; }

    public int? PredictedUsage { get; set; }

    public int? SafetyStock { get; set; }

    public int? ReorderPoint { get; set; }

    [StringLength(100)]
    public string? ForecastedBy { get; set; }

    [Range(0, 1)]
    public decimal? ForecastConfidence { get; set; }

    public DateTime? ForecastDate { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public string? SparepartName { get; set; }
    public string? CenterName { get; set; }
}