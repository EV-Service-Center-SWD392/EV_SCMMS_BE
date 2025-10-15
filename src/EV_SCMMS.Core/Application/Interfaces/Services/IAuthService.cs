using EV_SCMMS.Core.Application.DTOs.Auth;
using EV_SCMMS.Core.Application.DTOs.User;

using EV_SCMMS.Core.Application.Results;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Authentication result</returns>
    Task<IServiceResult<AuthResultDto>> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="registerDto">Registration data</param>
    /// <returns>Authentication result</returns>
    Task<IServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto);

    /// <summary>
    /// Create a new staff member (Admin only)
    /// </summary>
    /// <param name="createStaffDto">Staff creation data</param>
    /// <returns>Authentication result</returns>
    Task<IServiceResult<UserDto>> CreateStaffAsync(CreateStaffDto createStaffDto);

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New authentication result</returns>
    Task<IServiceResult<AuthResultDto>> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <returns>Success status</returns>
    Task<IServiceResult<bool>> RevokeTokenAsync(string refreshToken);
}