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
    /// <param name="accessToken">Current access token to validate</param>
    /// <returns>New authentication result</returns>
    Task<IServiceResult<AuthResultDto>> RefreshTokenAsync(string refreshToken, string accessToken);

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <returns>Success status</returns>
    Task<IServiceResult<bool>> RevokeTokenAsync(string refreshToken);

    /// <summary>
    /// Revoke all refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success status</returns>
    Task<IServiceResult<bool>> RevokeAllUserTokensAsync(Guid userId);
}