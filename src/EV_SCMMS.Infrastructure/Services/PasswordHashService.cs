using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace EV_SCMMS.Infrastructure.Services;

/// <summary>
/// Password hashing service implementation using ASP.NET Core Identity
/// </summary>
public class PasswordHashService : IPasswordHashService
{
    private readonly IPasswordHasher<object> _passwordHasher;

    public PasswordHashService()
    {
        _passwordHasher = new PasswordHasher<object>();
    }

    /// <summary>
    /// Hash password using ASP.NET Core Identity PasswordHasher
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(new object(), password);
    }

    /// <summary>
    /// Verify password against hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hashedPassword">Hashed password</param>
    /// <returns>True if password matches</returns>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(new object(), hashedPassword, password);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}