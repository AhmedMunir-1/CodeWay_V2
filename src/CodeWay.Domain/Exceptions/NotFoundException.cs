namespace CodeWay.Domain.Exceptions;

/// <summary>
/// Thrown when a requested resource cannot be found.
/// Caught by middleware → HTTP 404 Not Found.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with identifier '{key}' was not found.") { }

    public NotFoundException(string message)
        : base(message) { }
}
