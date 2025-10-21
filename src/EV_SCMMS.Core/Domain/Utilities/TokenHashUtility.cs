using System.Security.Cryptography;
using System.Text;

namespace EV_SCMMS.Core.Domain.Utilities;

/// <summary>
/// Utility class for token hashing operations
/// </summary>
public static class TokenHashUtility
{
    /// <summary>
    /// Computes SHA256 hash of a token and returns it as Base64 string
    /// </summary>
    /// <param name="token">Token to hash</param>
    /// <returns>Base64 encoded SHA256 hash</returns>
    public static string ComputeTokenHash(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }
}