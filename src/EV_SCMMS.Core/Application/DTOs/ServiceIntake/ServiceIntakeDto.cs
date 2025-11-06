namespace EV_SCMMS.Core.Application.DTOs.ServiceIntake;

/// <summary>
/// Response DTO for service intake lifecycle
/// </summary>
public class ServiceIntakeDto
{
    public Guid Id { get; set; }
    public Guid CenterId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid TechnicianId { get; set; }
    public Guid? AssignmentId { get; set; }
    public Guid? BookingId { get; set; }
    public int? Odometer { get; set; }
    public int? BatteryPercent { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
