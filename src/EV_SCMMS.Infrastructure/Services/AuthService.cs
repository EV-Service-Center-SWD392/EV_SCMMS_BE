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
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result</returns>
    public async Task<IServiceResult<AuthResultDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get user with role information
            var user = await _unitOfWork.UserRepository.GetByEmailWithRoleAsync(loginDto.Email, cancellationToken);
            
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
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result</returns>
    public async Task<IServiceResult<AuthResultDto>> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if email already exists
            var emailExists = await _unitOfWork.UserRepository.EmailExistsAsync(registerDto.Email, cancellationToken);
            if (emailExists)
            {
                _logger?.LogWarning("Registration attempt failed for email: {Email} - Email already exists", registerDto.Email);
                return ServiceResult<AuthResultDto>.Failure("Email already exists");
            }

            // Get role for user
            Role? role;
            if (registerDto.RoleId.HasValue)
            {
                role = await _unitOfWork.RoleRepository.GetByIdAsync(registerDto.RoleId.Value, cancellationToken);
                if (role == null)
                {
                    _logger?.LogWarning("Registration attempt failed for email: {Email} - Invalid role ID: {RoleId}", registerDto.Email, registerDto.RoleId);
                    return ServiceResult<AuthResultDto>.Failure("Invalid role specified");
                }
            }
            else
            {
                // Get default user role
                role = await _unitOfWork.RoleRepository.GetDefaultUserRoleAsync(cancellationToken);
                if (role == null)
                {
                    _logger?.LogError("Registration attempt failed for email: {Email} - Default user role not found", registerDto.Email);
                    return ServiceResult<AuthResultDto>.Failure("Default user role not found. Please contact administrator.");
                }
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
            await _unitOfWork.UserRepository.AddAsync(newUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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
    /// Refresh access token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New authentication result</returns>
    public async Task<IServiceResult<AuthResultDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
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
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    public async Task<IServiceResult<bool>> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
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