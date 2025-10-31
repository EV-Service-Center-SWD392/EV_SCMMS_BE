using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// JWT token service implementation
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryMinutes;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured");
        _issuer = _configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
        _audience = _configuration["JwtSettings:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
        _expiryMinutes = int.TryParse(_configuration["JwtSettings:ExpiryMinutes"], out var minutes) ? minutes : 60;
    }

    /// <summary>
    /// Generate JWT access token
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="email">User email</param>
    /// <param name="role">User role</param>
    /// <returns>JWT token</returns>
    public string GenerateAccessToken(Guid userId, string email, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Aud, _audience),
            new Claim(JwtRegisteredClaimNames.Iss, _issuer)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generate refresh token
    /// </summary>
    /// <returns>Refresh token</returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Get principal from expired token
    /// </summary>
    /// <param name="token">Expired JWT token</param>
    /// <returns>Claims principal</returns>
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    /// <summary>
    /// Validate token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if valid</returns>
    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get token expiration time
    /// </summary>
    /// <returns>Token expiration datetime</returns>
    public DateTime GetTokenExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_expiryMinutes);
    }
}