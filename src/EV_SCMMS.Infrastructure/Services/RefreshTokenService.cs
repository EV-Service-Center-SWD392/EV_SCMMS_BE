using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Core.Application.Services;
using EV_SCMMS.Core.Domain.Utilities;
using EV_SCMMS.Core.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EV_SCMMS.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly int _refreshTokenExpiryDays;

    public RefreshTokenService(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<RefreshTokenService> logger)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _logger = logger;
        _refreshTokenExpiryDays = int.TryParse(_configuration["JwtSettings:RefreshTokenExpiryDays"], out var days) ? days : 7;
    }

    public async Task<Refreshtoken> GenerateRefreshTokenAsync(Guid userId, string accessToken)
    {
        var accessTokenHash = TokenHashUtility.ComputeTokenHash(accessToken);
        _logger.LogDebug("Generating refresh token for user {UserId} with access token hash: {Hash}", 
            userId, accessTokenHash[..8] + "..."); // Log first 8 chars for debugging

        var refreshToken = new Refreshtoken
        {
            Userid = userId,
            Token = accessTokenHash,
            Expiresat = DateTime.Now.AddDays(_refreshTokenExpiryDays),
            Createdat = DateTime.Now
        };

        await _unitOfWork.RefreshTokenRepository.Add(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogDebug("Refresh token created with ID: {TokenId}", refreshToken.Tokenid);
        return refreshToken;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token)
    {
        if (!Guid.TryParse(token, out var tokenId))
            return false;

        var refreshToken = await _unitOfWork.RefreshTokenRepository.GetByTokenIdAsync(tokenId);
        return refreshToken != null;
    }

    public async Task<Refreshtoken?> GetRefreshTokenAsync(string token)
    {
        if (!Guid.TryParse(token, out var tokenId))
            return null;

        return await _unitOfWork.RefreshTokenRepository.GetByTokenIdWithUserAsync(tokenId);
    }

    public async Task<Refreshtoken?> GetRefreshTokenWithAccessTokenValidationAsync(string token, string accessToken)
    {
        if (!Guid.TryParse(token, out var tokenId))
            return null;

        var accessTokenHash = TokenHashUtility.ComputeTokenHash(accessToken);
        
        return await _unitOfWork.RefreshTokenRepository.GetByTokenIdAndAccessTokenHashAsync(tokenId, accessTokenHash);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token)
    {
        if (!Guid.TryParse(token, out var tokenId))
            return false;

        var refreshToken = await _unitOfWork.RefreshTokenRepository.GetByIdAsync(tokenId);

        if (refreshToken == null)
            return false;

        _unitOfWork.RefreshTokenRepository.Delete(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RevokeAllUserRefreshTokensAsync(Guid userId)
    {
        var userTokens = await _unitOfWork.RefreshTokenRepository.GetByUserIdAsync(userId);

        if (!userTokens.Any())
            return false;

        foreach (var token in userTokens)
        {
            _unitOfWork.RefreshTokenRepository.Delete(token);
        }
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsRefreshTokenActiveAsync(string token)
    {
        return await ValidateRefreshTokenAsync(token);
    }

    public async Task<bool> ValidateAccessTokenHashAsync(Guid userId, string accessTokenHash)
    {
        _logger.LogDebug("Validating access token hash for user {UserId}: {Hash}", 
            userId, accessTokenHash[..8] + "..."); // Log first 8 chars for debugging

        var result = await _unitOfWork.RefreshTokenRepository.ValidateAccessTokenHashAsync(userId, accessTokenHash);

        _logger.LogDebug("Access token hash validation result for user {UserId}: {Result}", userId, result);
        return result;
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _unitOfWork.RefreshTokenRepository.GetExpiredTokensAsync();

        if (expiredTokens.Any())
        {
            foreach (var token in expiredTokens)
            {
                _unitOfWork.RefreshTokenRepository.Delete(token);
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // public async Task<bool> IsRefreshTokenActiveAsync(string token)
    // {
    //     if (!Guid.TryParse(token, out var tokenId))
    //         return false;

    //     return await _context.ActiveRefreshTokens
    //         .AnyAsync(art => art.Tokenid == tokenId);
    // }

    // public async Task<bool> ValidateAccessTokenHashAsync(Guid userId, string accessTokenHash)
    // {
    //     _logger.LogDebug("Validating access token hash for user {UserId}: {Hash}", 
    //         userId, accessTokenHash[..8] + "..."); // Log first 8 chars for debugging

    //     var result = await _context.RefreshTokens
    //         .AnyAsync(rt => rt.Userid == userId && 
    //                        rt.Token == accessTokenHash && 
    //                        rt.Expiresat > DateTime.Now);

    //     _logger.LogDebug("Access token hash validation result for user {UserId}: {Result}", userId, result);
    //     return result;
    // }

    // public async Task CleanupExpiredTokensAsync()
    // {
    //     var expiredTokens = await _context.RefreshTokens
    //         .Where(rt => rt.Expiresat <= DateTime.Now)
    //         .ToListAsync();

    //     if (expiredTokens.Any())
    //     {
    //         _context.RefreshTokens.RemoveRange(expiredTokens);
    //         await _context.SaveChangesAsync();
    //     }
    // }
}