using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.ServiceIntake;

/// <summary>
/// Create request DTO for vehicle service intake workflow (ServiceIntakeThaoNTT)
/// </summary>
public class CreateServiceIntakeDto
{
    [Required]
    public Guid CenterId { get; set; }

    [Required]
    public Guid VehicleId { get; set; }

    [Required]
    public Guid TechnicianId { get; set; }

    public Guid? AssignmentId { get; set; }

    public Guid? BookingId { get; set; }

    public int? Odometer { get; set; }

    public int? BatteryPercent { get; set; }

    public string? Notes { get; set; }
}
