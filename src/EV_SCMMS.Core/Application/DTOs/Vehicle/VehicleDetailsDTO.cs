using System.ComponentModel.DataAnnotations;
using EV_SCMMS.Core.Application.DTOs.User;

public class VehicleDetailDto
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

  public UserDto Customer { get; set; } = null!;  // Nested Useraccount details
}
