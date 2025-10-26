using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.ServiceIntake;

/// <summary>
/// Create request DTO for vehicle service intake workflow (ServiceIntakeThaoNTT)
/// Simplified to avoid duplicating data derived from Booking/Assignment.
/// </summary>
public class CreateServiceIntakeDto
{
    [Required]
    public Guid BookingId { get; set; }

    public int? Odometer { get; set; }

    public int? BatteryPercent { get; set; }

    public string? Notes { get; set; }
}
