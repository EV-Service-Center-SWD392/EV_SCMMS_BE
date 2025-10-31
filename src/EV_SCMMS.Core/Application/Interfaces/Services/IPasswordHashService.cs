namespace EV_SCMMS.Core.Application.Interfaces.Services;

/// <summary>
/// Password hashing service interface
/// </summary>
public interface IPasswordHashService
{
    /// <summary>
    /// Hash password using BCrypt
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verify password against hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hashedPassword">Hashed password</param>
    /// <returns>True if password matches</returns>
    bool VerifyPassword(string password, string hashedPassword);
}