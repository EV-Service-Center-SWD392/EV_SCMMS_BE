using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Core.Application.Services;
using EV_SCMMS.Core.Domain.Utilities;
using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EV_SCMMS.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly int _refreshTokenExpiryDays;

    public RefreshTokenService(AppDbContext context, IConfiguration configuration, ILogger<RefreshTokenService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _refreshTokenExpiryDays = int.TryParse(_configuration["JwtSettings:RefreshTokenExpiryDays"], out var days) ? days : 7;
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, string accessToken)
    {
        var accessTokenHash = TokenHashUtility.ComputeTokenHash(accessToken);
        _logger.LogDebug("Generating refresh token for user {UserId} with access token hash: {Hash}", 
            userId, accessTokenHash[..8] + "..."); // Log first 8 chars for debugging

        var refreshToken = new RefreshToken
        {
            Userid = userId,
            Token = accessTokenHash,
            Expiresat = DateTime.Now.AddDays(_refreshTokenExpiryDays),
            Createdat = DateTime.Now
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        _logger.LogDebug("Refresh token created with ID: {TokenId}", refreshToken.Tokenid);
        return refreshToken;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token)
    {
        if (!Guid.TryParse(token, out var tokenId))
            return false;

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Tokenid == tokenId);

        return refreshToken != null && refreshToken.Expiresat > DateTime.Now;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        if (!Guid.TryParse(token, out var tokenId))
            return null;

        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Tokenid == tokenId && rt.Expiresat > DateTime.Now);
    }

    public async Task<RefreshToken?> GetRefreshTokenWithAccessTokenValidationAsync(string token, string accessToken)
    {
        if (!Guid.TryParse(token, out var tokenId))
            return null;

        var accessTokenHash = TokenHashUtility.ComputeTokenHash(accessToken);
        
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Tokenid == tokenId && 
                                     rt.Token == accessTokenHash && 
                                     rt.Expiresat > DateTime.Now);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token)
    {
        if (!Guid.TryParse(token, out var tokenId))
            return false;

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Tokenid == tokenId);

        if (refreshToken == null)
            return false;

        _context.RefreshTokens.Remove(refreshToken);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RevokeAllUserRefreshTokensAsync(Guid userId)
    {
        var userTokens = await _context.RefreshTokens
            .Where(rt => rt.Userid == userId)
            .ToListAsync();

        if (!userTokens.Any())
            return false;

        _context.RefreshTokens.RemoveRange(userTokens);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsRefreshTokenActiveAsync(string token)
    {
        if (!Guid.TryParse(token, out var tokenId))
            return false;

        return await _context.ActiveRefreshTokens
            .AnyAsync(art => art.Tokenid == tokenId);
    }

    public async Task<bool> ValidateAccessTokenHashAsync(Guid userId, string accessTokenHash)
    {
        _logger.LogDebug("Validating access token hash for user {UserId}: {Hash}", 
            userId, accessTokenHash[..8] + "..."); // Log first 8 chars for debugging

        var result = await _context.RefreshTokens
            .AnyAsync(rt => rt.Userid == userId && 
                           rt.Token == accessTokenHash && 
                           rt.Expiresat > DateTime.Now);

        _logger.LogDebug("Access token hash validation result for user {UserId}: {Result}", userId, result);
        return result;
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.Expiresat <= DateTime.Now)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}