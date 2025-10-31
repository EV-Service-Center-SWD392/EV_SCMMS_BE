using EV_SCMMS.Core.Domain.Models;

namespace EV_SCMMS.Core.Application.Services;

public interface IRefreshTokenService
{
    Task<Refreshtoken> GenerateRefreshTokenAsync(Guid userId, string accessToken);
    Task<bool> ValidateRefreshTokenAsync(string token);
    Task<Refreshtoken?> GetRefreshTokenAsync(string token);
    Task<Refreshtoken?> GetRefreshTokenWithAccessTokenValidationAsync(string token, string accessToken);
    Task<bool> RevokeRefreshTokenAsync(string token);
    Task<bool> RevokeAllUserRefreshTokensAsync(Guid userId);
    Task<bool> IsRefreshTokenActiveAsync(string token);
    Task<bool> ValidateAccessTokenHashAsync(Guid userId, string accessTokenHash);
    Task CleanupExpiredTokensAsync();
}