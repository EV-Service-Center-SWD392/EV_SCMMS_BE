namespace EV_SCMMS.Core.Application.DTOs.ServiceIntake;

/// <summary>
/// DTO to update mutable fields on an active intake
/// </summary>
public class UpdateServiceIntakeDto
{
    public int? Odometer { get; set; }

    public int? BatteryPercent { get; set; }
}
