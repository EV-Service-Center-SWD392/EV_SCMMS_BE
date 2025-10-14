using EV_SCMMS.Core.Application.DTOs.Auth;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_SCMMS.WebAPI.Controllers;

/// <summary>
/// Authentication controller for login, register, and token management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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
    /// Authenticate user with email and password
    /// </summary>
    /// <param name="loginDto">Login credentials containing email and password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with JWT token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Invalid login credentials</response>
    /// <response code="401">Authentication failed</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(loginDto, cancellationToken);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);
            return Ok(result);
        }

        _logger.LogWarning("Login failed for user: {Email} - {Error}", loginDto.Email, result.Message);
        return Unauthorized(result);
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="registerDto">Registration data including user details and credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with JWT token for the new user</returns>
    /// <response code="200">Registration successful</response>
    /// <response code="400">Invalid registration data</response>
    /// <response code="409">Email already exists</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(registerDto, cancellationToken);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);
            return Ok(result);
        }

        _logger.LogWarning("Registration failed for user: {Email} - {Error}", registerDto.Email, result.Message);
        
        // Return conflict status for email already exists
        if (result.Message.Contains("already exists"))
        {
            return Conflict(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Refresh JWT access token using refresh token
    /// </summary>
    /// <param name="refreshToken">Valid refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New authentication result with fresh JWT token</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="400">Invalid refresh token</response>
    /// <response code="401">Refresh token expired or invalid</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return BadRequest("Refresh token is required");
        }

        var result = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("Token refreshed successfully");
            return Ok(result);
        }

        _logger.LogWarning("Token refresh failed - {Error}", result.Message);
        return Unauthorized(result);
    }

    /// <summary>
    /// Revoke refresh token (logout)
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    /// <response code="200">Token revoked successfully</response>
    /// <response code="400">Invalid refresh token</response>
    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeTokenAsync([FromBody] string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return BadRequest("Refresh token is required");
        }

        var result = await _authService.RevokeTokenAsync(refreshToken, cancellationToken);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("Token revoked successfully");
            return Ok(result);
        }

        _logger.LogWarning("Token revocation failed - {Error}", result.Message);
        return BadRequest(result);
    }

    /// <summary>
    /// Get current user information from JWT token
    /// </summary>
    /// <returns>Current user information</returns>
    /// <response code="200">User information retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
        {
            return Unauthorized("Invalid token claims");
        }

        var userInfo = new
        {
            UserId = userId,
            Email = email,
            Role = role
        };

        return Ok(new { IsSuccess = true, Data = userInfo, Message = "User information retrieved successfully" });
    }
}