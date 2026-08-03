namespace CodeWay.Domain.ValueObjects;

using CodeWay.Domain.Common;
using CodeWay.Domain.Exceptions;

/// <summary>Full name value object combining first and last name.</summary>
public sealed class FullName : BaseValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    private FullName() { }  // EF Core ctor
    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static FullName Of(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty.");

        return new FullName(firstName.Trim(), lastName.Trim());
    }

    public string DisplayName => $"{FirstName} {LastName}";

    public override string ToString() => DisplayName;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}
