using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Vehicle
{
  /// <summary>
  /// Response DTO cho Vehicle details hoặc list (dùng trong mapper ToDto)
  /// </summary>
  public class VehicleDto
  {
    public Guid VehicleId { get; set; }

    public Guid CustomerId { get; set; }

    public int? ModelId { get; set; }

    public string LicensePlate { get; set; } = string.Empty;

    public int? Year { get; set; }

    public string? Color { get; set; }

    public string? Status { get; set; }  // ACTIVE/INACTIVE

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
  }
}
