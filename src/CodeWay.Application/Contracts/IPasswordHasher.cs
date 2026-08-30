namespace CodeWay.Application.Contracts;

/// <summary>
/// Contract for hashing and verifying passwords.
/// Implemented in Infrastructure by <c>BcryptPasswordHasher</c> using BCrypt.Net-Next.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Hashes a plain-text password.</summary>
    string Hash(string password);

    /// <summary>Verifies a plain-text password against a stored hash.</summary>
    bool Verify(string password, string hash);
}
