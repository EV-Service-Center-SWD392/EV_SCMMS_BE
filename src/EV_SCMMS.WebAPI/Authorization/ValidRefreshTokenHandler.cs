using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EV_SCMMS.WebAPI.Authorization;

/// <summary>
/// Authorization handler to validate that access token exists in refresh tokens
/// </summary>
public class ValidRefreshTokenHandler : AuthorizationHandler<ValidRefreshTokenRequirement>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ValidRefreshTokenHandler> _logger;

    public ValidRefreshTokenHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ValidRefreshTokenHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ValidRefreshTokenRequirement requirement)
    {
        try
        {
            // Get user ID from claims
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid user ID in token claims");
                context.Fail();
                return;
            }

            // Extract access token from Authorization header
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                _logger.LogWarning("HttpContext is null");
                context.Fail();
                return;
            }

            var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Invalid authorization header");
                context.Fail();
                return;
            }

            var accessToken = authHeader.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("Empty access token");
                context.Fail();
                return;
            }

            // Check if access token hash exists in refresh tokens for this user
            var accessTokenHash = TokenHashUtility.ComputeTokenHash(accessToken);
            _logger.LogDebug("Checking access token hash for user {UserId}: {Hash}", 
                userId, accessTokenHash[..8] + "..."); // Log first 8 chars for debugging

            var hasValidRefreshToken = await _unitOfWork.RefreshTokenService.ValidateAccessTokenHashAsync(userId, accessTokenHash);

            if (hasValidRefreshToken)
            {
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("Access token not found in refresh tokens for user: {UserId}", userId);
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during refresh token validation");
            context.Fail();
        }
    }
}