using EV_SCMMS.Core.Application.DTOs;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="registerDto">Registration data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with JWT token</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(registerDto, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with JWT token</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(loginDto, cancellationToken);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="refreshTokenDto">Refresh token data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New authentication tokens</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RefreshTokenAsync(refreshTokenDto, cancellationToken);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    /// <summary>
    /// Logout user (revoke refresh token)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not found" });

        var result = await _authService.RevokeTokenAsync(userId, cancellationToken);

        if (!result)
            return BadRequest(new { message = "Failed to logout" });

        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="currentPassword">Current password</param>
    /// <param name="newPassword">New password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto changePasswordDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not found" });

        var result = await _authService.ChangePasswordAsync(
            userId,
            changePasswordDto.CurrentPassword,
            changePasswordDto.NewPassword,
            cancellationToken);

        if (!result)
            return BadRequest(new { message = "Failed to change password" });

        return Ok(new { message = "Password changed successfully" });
    }

    /// <summary>
    /// Get current user info (test authentication)
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value);

        return Ok(new
        {
            id = userId,
            username,
            email,
            roles
        });
    }
}

/// <summary>
/// DTO for password change
/// </summary>
public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
