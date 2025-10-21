using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for RefreshToken entity
/// </summary>
public class RefreshTokenRepository : GenericRepository<Refreshtoken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get refresh token by token ID
    /// </summary>
    /// <param name="tokenId">Token ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token if found and not expired</returns>
    public async Task<Refreshtoken?> GetByTokenIdAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Refreshtoken>()
            .FirstOrDefaultAsync(rt => rt.Tokenid == tokenId && rt.Expiresat > DateTime.Now, cancellationToken);
    }

    /// <summary>
    /// Get refresh token with user by token ID
    /// </summary>
    /// <param name="tokenId">Token ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token with user if found and not expired</returns>
    public async Task<Refreshtoken?> GetByTokenIdWithUserAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Refreshtoken>()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Tokenid == tokenId && rt.Expiresat > DateTime.Now, cancellationToken);
    }

    /// <summary>
    /// Get refresh token by token ID and access token hash
    /// </summary>
    /// <param name="tokenId">Token ID</param>
    /// <param name="accessTokenHash">Access token hash</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token with user if found and not expired</returns>
    public async Task<Refreshtoken?> GetByTokenIdAndAccessTokenHashAsync(Guid tokenId, string accessTokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Refreshtoken>()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Tokenid == tokenId && 
                               rt.Token == accessTokenHash && 
                               rt.Expiresat > DateTime.Now, cancellationToken);
    }

    /// <summary>
    /// Get all refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user's refresh tokens</returns>
    public async Task<IEnumerable<Refreshtoken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Refreshtoken>()
            .Where(rt => rt.Userid == userId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Check if access token hash is valid for user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="accessTokenHash">Access token hash</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if valid</returns>
    public async Task<bool> ValidateAccessTokenHashAsync(Guid userId, string accessTokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Refreshtoken>()
            .AnyAsync(rt => rt.Userid == userId && 
                           rt.Token == accessTokenHash && 
                           rt.Expiresat > DateTime.Now, cancellationToken);
    }

    /// <summary>
    /// Get expired refresh tokens
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of expired tokens</returns>
    public async Task<IEnumerable<Refreshtoken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Refreshtoken>()
            .Where(rt => rt.Expiresat <= DateTime.Now)
            .ToListAsync(cancellationToken);
    }
}