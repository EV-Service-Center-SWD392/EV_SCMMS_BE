using AutoMapper;
using EV_SCMMS.Core.Application.DTOs;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Infrastructure.Identity;
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
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return ServiceResult<AuthResponseDto>.Failure("User with this email already exists");
            }

            existingUser = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return ServiceResult<AuthResponseDto>.Failure("User with this username already exists");
            }

            // Create new user using AutoMapper
            var user = _mapper.Map<ApplicationUser>(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return ServiceResult<AuthResponseDto>.Failure(
                    "Failed to create user",
                    result.Errors.Select(e => e.Description)
                );
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

            var authResponse = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiryTime(),
                User = _mapper.Map<UserInfoDto>(user)
            };

            // Map roles separately as they're not part of the entity
            authResponse.User.Roles = roles;

            return ServiceResult<AuthResponseDto>.Success(authResponse, "Registration successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return ServiceResult<AuthResponseDto>.Failure("An error occurred during registration", new[] { ex.Message });
        }
    }

    public async Task<IServiceResult<AuthResponseDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find user by email or username
            var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUsername)
                ?? await _userManager.FindByNameAsync(loginDto.EmailOrUsername);

            if (user == null)
            {
                return ServiceResult<AuthResponseDto>.Failure("Invalid credentials");
            }

            if (!user.IsActive)
            {
                return ServiceResult<AuthResponseDto>.Failure("Account is inactive");
            }

            // Check password
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!passwordValid)
            {
                // Increment failed login count
                await _userManager.AccessFailedAsync(user);

                if (await _userManager.IsLockedOutAsync(user))
                {
                    return ServiceResult<AuthResponseDto>.Failure("Your account has been locked due to multiple failed login attempts");
                }

                return ServiceResult<AuthResponseDto>.Failure("Invalid email/username or password");
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

            var authResponse = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiryTime(),
                User = _mapper.Map<UserInfoDto>(user)
            };

            // Map roles separately as they're not part of the entity
            authResponse.User.Roles = roles;

            return ServiceResult<AuthResponseDto>.Success(authResponse, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return ServiceResult<AuthResponseDto>.Failure("An error occurred during login", new[] { ex.Message });
        }
    }

    public async Task<IServiceResult<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken = default)
    {
        // Implementation for refresh token logic
        // This would validate the refresh token and generate new tokens
        throw new NotImplementedException("Refresh token logic to be implemented");
    }

    public async Task<IServiceResult> RevokeTokenAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Failure("User not found");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);

        return ServiceResult.Success("Token revoked successfully");
    }

    public async Task<IServiceResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Failure("User not found");
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        
        if (!result.Succeeded)
        {
            return ServiceResult.Failure("Failed to change password", result.Errors.Select(e => e.Description));
        }

        return ServiceResult.Success("Password changed successfully");
    }
}
