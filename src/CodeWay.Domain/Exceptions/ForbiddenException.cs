namespace CodeWay.Domain.Exceptions;

/// <summary>
/// Thrown when a user attempts an action they are not authorized to perform.
/// Caught by middleware → HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException()
        : base("You do not have permission to perform this action.") { }

    public ForbiddenException(string message)
        : base(message) { }
}
