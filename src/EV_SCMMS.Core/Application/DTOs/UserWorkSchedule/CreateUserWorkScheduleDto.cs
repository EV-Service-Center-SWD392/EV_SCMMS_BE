using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EV_SCMMS.Core.Application.DTOs.UserWorkSchedule;

/// <summary>
/// DTO for creating UserWorkSchedule
/// </summary>
public class CreateUserWorkScheduleDto
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string CenterName { get; set; } = string.Empty;

    [Required]
    public string Shift { get; set; } = string.Empty;
    
    [Required]
    public DateTime WorkDate { get; set; }
    
    [JsonIgnore]
    public Guid? WorkScheduleId { get; set; }
}