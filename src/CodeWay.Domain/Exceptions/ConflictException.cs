namespace CodeWay.Domain.Exceptions;

/// <summary>
/// Thrown when a resource already exists or a conflict state is detected.
/// Caught by middleware → HTTP 409 Conflict.
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string resourceName, string field, object value)
        : base($"{resourceName} with {field} '{value}' already exists.") { }

    public ConflictException(string message)
        : base(message) { }
}
