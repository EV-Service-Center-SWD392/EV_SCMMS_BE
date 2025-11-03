namespace EV_SCMMS.Core.Application.DTOs.UserRole;

public class UpdateRoleDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public bool IsActive { get; set; } = true;
}