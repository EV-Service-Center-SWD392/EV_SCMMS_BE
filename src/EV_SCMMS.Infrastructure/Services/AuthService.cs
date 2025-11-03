using EV_SCMMS.Core.Application.DTOs.Auth;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
using EV_SCMMS.Core.Application.DTOs.User;
using EV_SCMMS.Infrastructure.Mappings;
using Microsoft.Extensions.Logging;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IPasswordHashService _passwordHashService;
  private readonly ITokenService _tokenService;
  private readonly ILogger<AuthService> _logger;

  public AuthService(
      IUnitOfWork unitOfWork,
      IPasswordHashService passwordHashService,
      ITokenService tokenService,
      ILogger<AuthService> logger)
  {
    _unitOfWork = unitOfWork;
    _passwordHashService = passwordHashService;
    _tokenService = tokenService;
    _logger = logger;
  }

  /// <summary>
  /// Authenticate user with email and password
  /// </summary>
  /// <param name="loginDto">Login credentials</param>
  /// <returns>Authentication result</returns>
  public async Task<IServiceResult<AuthResultDto>> LoginAsync(LoginDto loginDto)
  {
    try
    {
      // Get user with role information
      var user = await _unitOfWork.UserRepository.GetByEmailWithRoleAsync(loginDto.Email);

      if (user == null)
      {
        _logger?.LogWarning("Login attempt failed for email: {Email} - Useraccount not found", loginDto.Email);
        return ServiceResult<AuthResultDto>.Failure("Invalid email or password");
      }

      if (user.Isactive != true)
      {
        _logger?.LogWarning("Login attempt failed for email: {Email} - Useraccount is inactive", loginDto.Email);
        return ServiceResult<AuthResultDto>.Failure("Useraccount account is inactive");
      }

      // Verify password
      if (!_passwordHashService.VerifyPassword(loginDto.Password, user.Password))
      {
        _logger?.LogWarning("Login attempt failed for email: {Email} - Invalid password", loginDto.Email);
        return ServiceResult<AuthResultDto>.Failure("Invalid email or password");
      }

      // Generate tokens
      var accessToken = _tokenService.GenerateAccessToken(user.Userid, user.Email, user.Role.Name);
      var refreshTokenEntity = await _unitOfWork.RefreshTokenService.GenerateRefreshTokenAsync(user.Userid, accessToken);
      var expiresAt = _tokenService.GetTokenExpiration();

      // Create auth result
      var authResult = new AuthResultDto
      {
        UserId = user.Userid,
        Email = user.Email,
        FullName = $"{user.Firstname} {user.Lastname}".Trim(),
        Role = user.Role.Name,
        AccessToken = accessToken,
        RefreshToken = refreshTokenEntity.Tokenid.ToString(), // Return tokenid as refresh token
        ExpiresAt = expiresAt
      };

      _logger?.LogInformation("Useraccount logged in successfully: {Email}", loginDto.Email);
      return ServiceResult<AuthResultDto>.Success(authResult, "Login successful");
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
      return ServiceResult<AuthResultDto>.Failure($"Error during login: {ex.Message}");
    }
  }

  /// <summary>
  /// Register a new user
  /// </summary>
  /// <param name="registerDto">Registration data</param>
  /// <returns>Authentication result</returns>
  public async Task<IServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto)
  {
    try
    {
      // Check if email already exists
      var emailExists = await _unitOfWork.UserRepository.EmailExistsAsync(registerDto.Email);
      var phoneExists = await _unitOfWork.UserRepository.PhoneExistsAsync(registerDto.PhoneNumber);
      if (emailExists)
      {
        _logger?.LogWarning("Registration attempt failed for email: {Email} - Email already exists", registerDto.Email);
        return ServiceResult<UserDto>.Failure("Email already exists");
      }
      if (phoneExists)
      {
        _logger?.LogWarning("Registration attempt failed for Phone: {phone} - Phone already exists", registerDto.Email);
        return ServiceResult<UserDto>.Failure("Phone already exists");
      }
      // Get customer role by name (default for regular registration)
      var role = await _unitOfWork.RoleRepository.GetByNameAsync("CUSTOMER");
      if (role == null)
      {
        _logger?.LogError("Registration attempt failed for email: {Email} - Customer role not found", registerDto.Email);
        return ServiceResult<UserDto>.Failure("Customer role not found. Please contact administrator.");
      }

      // Hash password
      var hashedPassword = _passwordHashService.HashPassword(registerDto.Password);

      var newUser = registerDto.ToEntity();
      newUser.Roleid = role.Roleid;
      newUser.Password = hashedPassword;

      // Save user to database
      await _unitOfWork.UserRepository.AddAsync(newUser);
      await _unitOfWork.SaveChangesAsync();

      // Generate tokens
      var expiresAt = _tokenService.GetTokenExpiration();

      // Create auth result

        var authResult = newUser.ToDto();
      _logger?.LogInformation("Useraccount registered successfully: {Email}", registerDto.Email);
      return ServiceResult<UserDto>.Success(authResult, "Registration successful");
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "Error during registration for email: {Email}", registerDto.Email);
      return ServiceResult<UserDto>.Failure($"Error during registration: {ex.InnerException?.Message}");
    }
  }

  /// <summary>
  /// Create a new staff member (Admin only)
  /// </summary>
  /// <param name="createStaffDto">Staff creation data</param>
  /// <returns>Authentication result</returns>
  public async Task<IServiceResult<UserDto>> CreateStaffAsync(CreateStaffDto createStaffDto)
  {
    try
    {
      // Check if email already exists
      var emailExists = await _unitOfWork.UserRepository.EmailExistsAsync(createStaffDto.Email);
      if (emailExists)
      {
        _logger?.LogWarning("Staff creation attempt failed for email: {Email} - Email already exists", createStaffDto.Email);
        return ServiceResult<UserDto>.Failure("Email already exists");
      }

      // Find role by name
      var role = await _unitOfWork.RoleRepository.GetByNameAsync(createStaffDto.Role);
      if (role == null)
      {
        _logger?.LogWarning("Staff creation attempt failed for email: {Email} - Role not found: {Role}", createStaffDto.Email, createStaffDto.Role);
        return ServiceResult<UserDto>.Failure($"Role '{createStaffDto.Role}' not found");
      }

      // Hash password
      var hashedPassword = _passwordHashService.HashPassword(createStaffDto.Password);


      // Create new staff user
      var newUser = createStaffDto.CreateStaffToEntity();
      newUser.Roleid = role.Roleid;
      newUser.Password = hashedPassword;
      // Save user to database
      await _unitOfWork.UserRepository.AddAsync(newUser);
      await _unitOfWork.SaveChangesAsync();

      // Generate tokens
      var expiresAt = _tokenService.GetTokenExpiration();

      var authResult = newUser.ToDto();
      _logger?.LogInformation("Staff member created successfully: {Email} with role: {Role}", createStaffDto.Email, createStaffDto.Role);
      return ServiceResult<UserDto>.Success(authResult, "Staff member created successfully");
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "Error during staff creation for email: {Email}", createStaffDto.Email);
      return ServiceResult<UserDto>.Failure($"Error during staff creation: {ex.Message}");
    }
  }

  /// <summary>
  /// Refresh access token
  /// </summary>
  /// <param name="refreshToken">Refresh token</param>
  /// <param name="accessToken">Current access token to validate</param>
  /// <returns>New authentication result</returns>
  public async Task<IServiceResult<AuthResultDto>> RefreshTokenAsync(string refreshToken, string accessToken)
  {
    try
    {
      // Validate and get refresh token from database with access token validation
      var refreshTokenEntity = await _unitOfWork.RefreshTokenService.GetRefreshTokenWithAccessTokenValidationAsync(refreshToken, accessToken);
      
      if (refreshTokenEntity == null)
      {
        _logger?.LogWarning("Refresh token not found, expired, or access token mismatch: {RefreshToken}", refreshToken);
        return ServiceResult<AuthResultDto>.Failure("Invalid or expired refresh token, or access token mismatch");
      }

      // Get user with role information
      var user = await _unitOfWork.UserRepository.GetByIdWithRoleAsync(refreshTokenEntity.Userid);
      
      if (user == null || user.Isactive != true)
      {
        _logger?.LogWarning("Useraccount not found or inactive for refresh token: {UserId}", refreshTokenEntity.Userid);
        await _unitOfWork.RefreshTokenService.RevokeRefreshTokenAsync(refreshToken);
        return ServiceResult<AuthResultDto>.Failure("Useraccount account is not active");
      }

      // Generate new tokens
      var newAccessToken = _tokenService.GenerateAccessToken(user.Userid, user.Email, user.Role.Name);
      var newRefreshTokenEntity = await _unitOfWork.RefreshTokenService.GenerateRefreshTokenAsync(user.Userid, newAccessToken);
      var expiresAt = _tokenService.GetTokenExpiration();

      // Revoke the old refresh token
      await _unitOfWork.RefreshTokenService.RevokeRefreshTokenAsync(refreshToken);

      // Create auth result
      var authResult = new AuthResultDto
      {
        UserId = user.Userid,
        Email = user.Email,
        FullName = $"{user.Firstname} {user.Lastname}".Trim(),
        Role = user.Role.Name,
        AccessToken = newAccessToken,
        RefreshToken = newRefreshTokenEntity.Tokenid.ToString(), // Return tokenid as refresh token
        ExpiresAt = expiresAt
      };

      _logger?.LogInformation("Token refreshed successfully for user: {UserId}", user.Userid);
      return ServiceResult<AuthResultDto>.Success(authResult, "Token refreshed successfully");
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "Error during token refresh");
      return ServiceResult<AuthResultDto>.Failure($"Error during token refresh: {ex.Message}");
    }
  }

  /// <summary>
  /// Revoke refresh token
  /// </summary>
  /// <param name="refreshToken">Refresh token to revoke</param>
  /// <returns>Success status</returns>
  public async Task<IServiceResult<bool>> RevokeTokenAsync(string refreshToken)
  {
    try
    {
      var result = await _unitOfWork.RefreshTokenService.RevokeRefreshTokenAsync(refreshToken);
      
      if (!result)
      {
        _logger?.LogWarning("Attempted to revoke non-existent refresh token");
        return ServiceResult<bool>.Failure("Invalid refresh token");
      }

      _logger?.LogInformation("Refresh token revoked successfully");
      return ServiceResult<bool>.Success(true, "Token revoked successfully");
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "Error during token revocation");
      return ServiceResult<bool>.Failure($"Error during token revocation: {ex.Message}");
    }
  }

  /// <summary>
  /// Revoke all refresh tokens for a user
  /// </summary>
  /// <param name="userId">Useraccount ID</param>
  /// <returns>Success status</returns>
  public async Task<IServiceResult<bool>> RevokeAllUserTokensAsync(Guid userId)
  {
    try
    {
      var result = await _unitOfWork.RefreshTokenService.RevokeAllUserRefreshTokensAsync(userId);
      
      if (!result)
      {
        _logger?.LogWarning("No tokens found to revoke for user: {UserId}", userId);
        return ServiceResult<bool>.Success(true, "No tokens to revoke");
      }

      _logger?.LogInformation("All refresh tokens revoked successfully for user: {UserId}", userId);
      return ServiceResult<bool>.Success(true, "All tokens revoked successfully");
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "Error during all tokens revocation for user: {UserId}", userId);
      return ServiceResult<bool>.Failure($"Error during tokens revocation: {ex.Message}");
    }
  }
}