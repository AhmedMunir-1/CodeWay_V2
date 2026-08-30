namespace CodeWay.Application.Contracts;

/// <summary>
/// Provides the identity of the currently authenticated user from the HTTP context.
/// Implemented in Infrastructure by <c>CurrentUserService</c> using <c>IHttpContextAccessor</c>.
/// Controllers must NEVER be trusted for the current user's ID — always use this service.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// The <see cref="Guid"/> of the currently authenticated user.
    /// Returns <see langword="null"/> for unauthenticated requests.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// The email of the currently authenticated user.
    /// Returns <see langword="null"/> for unauthenticated requests.
    /// </summary>
    string? Email { get; }

    /// <summary>Whether the current request is authenticated.</summary>
    bool IsAuthenticated { get; }
}
