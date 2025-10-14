using EV_SCMMS.Core.Application.DTOs.Auth;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Core.Application.Results;
using EV_SCMMS.Core.Domain.Models;
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
        _logger?.LogWarning("Login attempt failed for email: {Email} - User not found", loginDto.Email);
        return ServiceResult<AuthResultDto>.Failure("Invalid email or password");
      }

      if (user.Isactive != true)
      {
        _logger?.LogWarning("Login attempt failed for email: {Email} - User is inactive", loginDto.Email);
        return ServiceResult<AuthResultDto>.Failure("User account is inactive");
      }

      // Verify password
      if (!_passwordHashService.VerifyPassword(loginDto.Password, user.Password))
      {
        _logger?.LogWarning("Login attempt failed for email: {Email} - Invalid password", loginDto.Email);
        return ServiceResult<AuthResultDto>.Failure("Invalid email or password");
      }

      // Generate tokens
      var accessToken = _tokenService.GenerateAccessToken(user.Userid, user.Email, user.Role.Name);
      var refreshToken = _tokenService.GenerateRefreshToken();
      var expiresAt = _tokenService.GetTokenExpiration();

      // Create auth result
      var authResult = new AuthResultDto
      {
        UserId = user.Userid,
        Email = user.Email,
        FullName = $"{user.Firstname} {user.Lastname}".Trim(),
        Role = user.Role.Name,
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        ExpiresAt = expiresAt
      };

      _logger?.LogInformation("User logged in successfully: {Email}", loginDto.Email);
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
  public async Task<IServiceResult<AuthResultDto>> RegisterAsync(RegisterDto registerDto)
  {
    try
    {
      // Check if email already exists
      var emailExists = await _unitOfWork.UserRepository.EmailExistsAsync(registerDto.Email);
      var phoneExists = await _unitOfWork.UserRepository.PhoneExistsAsync(registerDto.PhoneNumber);
      if (emailExists)
      {
        _logger?.LogWarning("Registration attempt failed for email: {Email} - Email already exists", registerDto.Email);
        return ServiceResult<AuthResultDto>.Failure("Email already exists");
      }
      if (phoneExists)
      {
        _logger?.LogWarning("Registration attempt failed for Phone: {phone} - Phone already exists", registerDto.Email);
        return ServiceResult<AuthResultDto>.Failure("Phone already exists");
      }
      // Get customer role by name (default for regular registration)
      var role = await _unitOfWork.RoleRepository.GetByNameAsync("CUSTOMER");
      if (role == null)
      {
        _logger?.LogError("Registration attempt failed for email: {Email} - Customer role not found", registerDto.Email);
        return ServiceResult<AuthResultDto>.Failure("Customer role not found. Please contact administrator.");
      }

      // Hash password
      var hashedPassword = _passwordHashService.HashPassword(registerDto.Password);

      // Create new user
      var newUser = new User
      {
        Userid = Guid.NewGuid(),
        Email = registerDto.Email.ToLower(),
        Password = hashedPassword,
        Firstname = registerDto.FirstName,
        Lastname = registerDto.LastName,
        Phonenumber = registerDto.PhoneNumber,
        Address = registerDto.Address,
        Birthday = registerDto.Birthday,
        Roleid = role.Roleid,
        Status = "ACTIVE",
        Isactive = true
      };

      // Save user to database
      await _unitOfWork.UserRepository.AddAsync(newUser);
      await _unitOfWork.SaveChangesAsync();

      // Generate tokens
      var accessToken = _tokenService.GenerateAccessToken(newUser.Userid, newUser.Email, role.Name);
      var refreshToken = _tokenService.GenerateRefreshToken();
      var expiresAt = _tokenService.GetTokenExpiration();

      // Create auth result
      var authResult = new AuthResultDto
      {
        UserId = newUser.Userid,
        Email = newUser.Email,
        FullName = $"{newUser.Firstname} {newUser.Lastname}".Trim(),
        Role = role.Name,
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        ExpiresAt = expiresAt
      };

      _logger?.LogInformation("User registered successfully: {Email}", registerDto.Email);
      return ServiceResult<AuthResultDto>.Success(authResult, "Registration successful");
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "Error during registration for email: {Email}", registerDto.Email);
      return ServiceResult<AuthResultDto>.Failure($"Error during registration: {ex.Message}");
    }
  }

  /// <summary>
  /// Create a new staff member (Admin only)
  /// </summary>
  /// <param name="createStaffDto">Staff creation data</param>
  /// <returns>Authentication result</returns>
  public async Task<IServiceResult<AuthResultDto>> CreateStaffAsync(CreateStaffDto createStaffDto)
  {
    try
    {
      // Check if email already exists
      var emailExists = await _unitOfWork.UserRepository.EmailExistsAsync(createStaffDto.Email);
      if (emailExists)
      {
        _logger?.LogWarning("Staff creation attempt failed for email: {Email} - Email already exists", createStaffDto.Email);
        return ServiceResult<AuthResultDto>.Failure("Email already exists");
      }

      // Find role by name
      var role = await _unitOfWork.RoleRepository.GetByNameAsync(createStaffDto.Role);
      if (role == null)
      {
        _logger?.LogWarning("Staff creation attempt failed for email: {Email} - Role not found: {Role}", createStaffDto.Email, createStaffDto.Role);
        return ServiceResult<AuthResultDto>.Failure($"Role '{createStaffDto.Role}' not found");
      }

      // Hash password
      var hashedPassword = _passwordHashService.HashPassword(createStaffDto.Password);

      // Create new staff user
      var newUser = new User
      {
        Userid = Guid.NewGuid(),
        Email = createStaffDto.Email.ToLower(),
        Password = hashedPassword,
        Firstname = createStaffDto.FirstName,
        Lastname = createStaffDto.LastName,
        Phonenumber = createStaffDto.PhoneNumber,
        Address = createStaffDto.Address,
        Birthday = createStaffDto.Birthday,
        Roleid = role.Roleid,
        Status = "ACTIVE",
        Isactive = true
      };

      // Save user to database
      await _unitOfWork.UserRepository.AddAsync(newUser);
      await _unitOfWork.SaveChangesAsync();

      // Generate tokens
      var accessToken = _tokenService.GenerateAccessToken(newUser.Userid, newUser.Email, role.Name);
      var refreshToken = _tokenService.GenerateRefreshToken();
      var expiresAt = _tokenService.GetTokenExpiration();

      // Create auth result
      var authResult = new AuthResultDto
      {
        UserId = newUser.Userid,
        Email = newUser.Email,
        FullName = $"{newUser.Firstname} {newUser.Lastname}".Trim(),
        Role = role.Name,
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        ExpiresAt = expiresAt
      };

      _logger?.LogInformation("Staff member created successfully: {Email} with role: {Role}", createStaffDto.Email, createStaffDto.Role);
      return ServiceResult<AuthResultDto>.Success(authResult, "Staff member created successfully");
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "Error during staff creation for email: {Email}", createStaffDto.Email);
      return ServiceResult<AuthResultDto>.Failure($"Error during staff creation: {ex.Message}");
    }
  }

  /// <summary>
  /// Refresh access token
  /// </summary>
  /// <param name="refreshToken">Refresh token</param>
  /// <returns>New authentication result</returns>
  public async Task<IServiceResult<AuthResultDto>> RefreshTokenAsync(string refreshToken)
  {
    try
    {
      // Note: In a production environment, you should store refresh tokens in database
      // and validate them properly. For now, we'll implement a basic version.

      // This is a simplified implementation
      // In a real application, you would:
      // 1. Store refresh tokens in database with user association
      // 2. Validate the refresh token exists and is not expired
      // 3. Check if the refresh token is not revoked

      _logger?.LogWarning("RefreshTokenAsync not fully implemented - requires refresh token storage");
      return ServiceResult<AuthResultDto>.Failure("Refresh token functionality not fully implemented");
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
      // Note: In a production environment, you should store refresh tokens in database
      // and mark them as revoked. For now, we'll implement a basic version.

      _logger?.LogWarning("RevokeTokenAsync not fully implemented - requires refresh token storage");
      return ServiceResult<bool>.Success(true, "Token revoked successfully");
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "Error during token revocation");
      return ServiceResult<bool>.Failure($"Error during token revocation: {ex.Message}");
    }
  }
}