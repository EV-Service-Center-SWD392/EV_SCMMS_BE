using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.User;

/// <summary>
/// DTO for updating user role
/// </summary>
public class UpdateUserRoleDto
{
    [Required]
    public Guid RoleId { get; set; }
}