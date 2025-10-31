using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for RefreshToken entity operations
/// </summary>
public interface IRefreshTokenRepository : IGenericRepository<Refreshtoken>
{
    /// <summary>
    /// Get refresh token by token ID
    /// </summary>
    /// <param name="tokenId">Token ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token if found and not expired</returns>
    Task<Refreshtoken?> GetByTokenIdAsync(Guid tokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get refresh token with user by token ID
    /// </summary>
    /// <param name="tokenId">Token ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token with user if found and not expired</returns>
    Task<Refreshtoken?> GetByTokenIdWithUserAsync(Guid tokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get refresh token by token ID and access token hash
    /// </summary>
    /// <param name="tokenId">Token ID</param>
    /// <param name="accessTokenHash">Access token hash</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token with user if found and not expired</returns>
    Task<Refreshtoken?> GetByTokenIdAndAccessTokenHashAsync(Guid tokenId, string accessTokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user's refresh tokens</returns>
    Task<IEnumerable<Refreshtoken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if access token hash is valid for user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="accessTokenHash">Access token hash</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if valid</returns>
    Task<bool> ValidateAccessTokenHashAsync(Guid userId, string accessTokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get expired refresh tokens
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of expired tokens</returns>
    Task<IEnumerable<Refreshtoken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default);
}