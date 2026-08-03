namespace CodeWay.Domain.Exceptions;

/// <summary>
/// Base exception for all domain rule violations.
/// Thrown inside entity/aggregate methods when business invariants are broken.
/// Caught by <c>ExceptionHandlingMiddleware</c> in the API layer → HTTP 400.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
