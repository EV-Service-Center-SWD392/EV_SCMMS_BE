using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.SparepartForecast;

/// <summary>
/// SparepartForecast update request DTO
/// </summary>
public class UpdateSparepartForecastDto
{
    [Required]
    public Guid SparepartId { get; set; }

    [Required]
    public Guid CenterId { get; set; }

    [Range(0, int.MaxValue)]
    public int? PredictedUsage { get; set; }

    [Range(0, int.MaxValue)]
    public int? SafetyStock { get; set; }

    [Range(0, int.MaxValue)]
    public int? ReorderPoint { get; set; }

    [StringLength(100)]
    public string? ForecastedBy { get; set; }

    [Range(0, 1)]
    public decimal? ForecastConfidence { get; set; }

    public DateTime? ForecastDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }
    public Guid ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
}