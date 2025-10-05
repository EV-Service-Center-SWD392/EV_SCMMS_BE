using EV_SCMMS.Core.Application.DTOs;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "User with this email already exists",
                    Errors = new[] { "Email is already registered" }
                };
            }

            existingUser = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "User with this username already exists",
                    Errors = new[] { "Username is already taken" }
                };
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Failed to create user",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            _logger.LogInformation("User {Username} registered successfully", user.UserName);

            // Generate tokens
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateAccessToken(user.Id, user.UserName!, user.Email!, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResultDto
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiryTime(),
                Message = "Registration successful",
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Username = user.UserName!,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Roles = roles
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return new AuthResultDto
            {
                Success = false,
                Message = "An error occurred during registration",
                Errors = new[] { ex.Message }
            };
        }
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find user by email or username
            var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUsername)
                ?? await _userManager.FindByNameAsync(loginDto.EmailOrUsername);

            if (user == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Errors = new[] { "User not found" }
                };
            }

            if (!user.IsActive)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Account is inactive",
                    Errors = new[] { "Your account has been deactivated" }
                };
            }

            // Check password
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!passwordValid)
            {
                // Increment failed login count
                await _userManager.AccessFailedAsync(user);

                if (await _userManager.IsLockedOutAsync(user))
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Account locked",
                        Errors = new[] { "Your account has been locked due to multiple failed login attempts" }
                    };
                }

                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Errors = new[] { "Invalid email/username or password" }
                };
            }

            // Reset failed login count on successful login
            await _userManager.ResetAccessFailedCountAsync(user);

            // Generate tokens
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateAccessToken(user.Id, user.UserName!, user.Email!, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User {Username} logged in successfully", user.UserName);

            return new AuthResultDto
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiryTime(),
                Message = "Login successful",
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Username = user.UserName!,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Roles = roles
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return new AuthResultDto
            {
                Success = false,
                Message = "An error occurred during login",
                Errors = new[] { ex.Message }
            };
        }
    }

    public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken = default)
    {
        // Implementation for refresh token logic
        // This would validate the refresh token and generate new tokens
        throw new NotImplementedException("Refresh token logic to be implemented");
    }

    public async Task<bool> RevokeTokenAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }
}
