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
    public Guid CenterId { get; set; }

    [Required]
    public string Shift { get; set; } = string.Empty;
    [JsonIgnore]
    public Guid? WorkScheduleId { get; set; }
}