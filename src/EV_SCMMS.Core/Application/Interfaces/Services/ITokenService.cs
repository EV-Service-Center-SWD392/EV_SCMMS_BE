namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// JWT token service interface
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(string userId, string username, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    DateTime GetTokenExpiryTime();
}
