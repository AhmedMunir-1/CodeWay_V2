namespace CodeWay.Application.Contracts;

/// <summary>
/// Contract for generating and validating stateless data-protection tokens
/// (password reset, email confirmation). Implemented in Infrastructure using
/// ASP.NET Core <c>IDataProtector</c> — no database storage required.
/// </summary>
public interface IDataProtectorTokenService
{
    /// <summary>Generates a URL-safe token for the given purpose and payload.</summary>
    string GenerateToken(string purpose, string payload);

    /// <summary>
    /// Validates and decodes a token for the given purpose.
    /// Returns the original payload on success, or <see langword="null"/> if
    /// the token is invalid, tampered, or expired.
    /// </summary>
    string? ValidateToken(string purpose, string token);
}
