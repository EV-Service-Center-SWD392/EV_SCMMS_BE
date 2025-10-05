using EV_SCMMS.Core.Application.DTOs;
using EV_SCMMS.Core.Application.Results;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// User service interface for business logic operations
/// </summary>
public interface IUserService
{
    Task<ServiceResult<UserDto>> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<IEnumerable<UserDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<UserDto>> CreateUserAsync(UserDto userDto, CancellationToken cancellationToken = default);
    Task<ServiceResult<UserDto>> UpdateUserAsync(int id, UserDto userDto, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
}
