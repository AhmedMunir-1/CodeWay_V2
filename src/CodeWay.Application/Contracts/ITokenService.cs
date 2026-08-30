namespace CodeWay.Application.Contracts;

using CodeWay.Domain.Entities.Identity;

/// <summary>
/// Contract for generating and validating JWT access tokens and refresh tokens.
/// Implemented in Infrastructure by <c>JwtTokenService</c>.
/// </summary>
public interface ITokenService
{
    /// <summary>Generates a signed JWT access token for the given user.</summary>
    string GenerateAccessToken(User user);

    /// <summary>Generates a cryptographically-random refresh token string.</summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Extracts the user's <see cref="Guid"/> from a token string without validating
    /// expiry — used during refresh rotation where the access token may be expired.
    /// Returns null if the token cannot be parsed.
    /// </summary>
    Guid? GetUserIdFromToken(string token);
}
