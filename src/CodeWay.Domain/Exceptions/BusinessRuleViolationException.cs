namespace CodeWay.Domain.Exceptions;

/// <summary>
/// Thrown when a business invariant or rule is violated within the domain.
/// More specific than <see cref="DomainException"/> — use this when the cause
/// is a clearly named business rule (e.g. "Cannot publish a course with no lessons").
/// Caught by middleware → HTTP 422 Unprocessable Entity.
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    public string RuleName { get; }

    public BusinessRuleViolationException(string ruleName, string message)
        : base(message)
    {
        RuleName = ruleName;
    }
}
