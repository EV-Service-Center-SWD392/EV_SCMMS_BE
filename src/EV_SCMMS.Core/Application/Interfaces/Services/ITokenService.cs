using System.Security.Claims;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// JWT token service interface
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate JWT access token
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="email">User email</param>
    /// <param name="role">User role</param>
    /// <returns>JWT token</returns>
    string GenerateAccessToken(Guid userId, string email, string role);

    /// <summary>
    /// Generate refresh token
    /// </summary>
    /// <returns>Refresh token</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Get principal from expired token
    /// </summary>
    /// <param name="token">Expired JWT token</param>
    /// <returns>Claims principal</returns>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    /// <summary>
    /// Validate token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if valid</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Get token expiration time
    /// </summary>
    /// <returns>Token expiration datetime</returns>
    DateTime GetTokenExpiration();
}