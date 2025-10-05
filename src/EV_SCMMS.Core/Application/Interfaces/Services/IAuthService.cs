using EV_SCMMS.Core.Application.DTOs;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken = default);
    Task<bool> RevokeTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}
