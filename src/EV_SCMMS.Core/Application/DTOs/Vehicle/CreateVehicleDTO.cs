using System.ComponentModel.DataAnnotations;

public class CreateVehicleDto
{

  public int ModelId { get; set; }

  public string LicensePlate { get; set; } = string.Empty;

  public int? Year { get; set; }

  public string? Color { get; set; }

  public string? Status { get; set; }  // Validate ACTIVE/INACTIVE

  public bool? IsActive { get; set; }
}
