using EV_SCMMS.Core.Application.DTOs.UserRole;
using EV_SCMMS.Core.Application.Interfaces;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface IUserRoleService
{
    Task<IServiceResult<UserRoleDto>> CreateAsync(CreateUserRoleDto dto);
    Task<IServiceResult<UserRoleDto>> UpdateAsync(Guid id, UpdateRoleDto dto);
    Task<IServiceResult<bool>> DeleteAsync(Guid id);
    Task<IServiceResult<UserRoleDto>> GetByIdAsync(Guid id);
    Task<IServiceResult<List<UserRoleDto>>> GetAllAsync();
    Task<IServiceResult<List<UserRoleDto>>> GetActiveRolesAsync();
}