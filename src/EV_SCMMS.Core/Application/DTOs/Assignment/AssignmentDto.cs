namespace EV_SCMMS.Core.Application.DTOs.Assignment;

/// <summary>
/// Response DTO for technician assignment (AssignmentThaoNTT)
/// </summary>
public class AssignmentDto
{
    public Guid Id { get; set; }
    public Guid TechnicianId { get; set; }
    public Guid CenterId { get; set; }
    public DateTime PlannedStartUtc { get; set; }
    public DateTime PlannedEndUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
}
