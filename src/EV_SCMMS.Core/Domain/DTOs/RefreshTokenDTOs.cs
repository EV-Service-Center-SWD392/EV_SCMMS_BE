namespace EV_SCMMS.Core.Domain.DTOs;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
}

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

public class RevokeTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}