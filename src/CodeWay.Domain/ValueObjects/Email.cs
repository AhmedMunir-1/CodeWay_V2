namespace CodeWay.Domain.ValueObjects;

using System.Text.RegularExpressions;
using CodeWay.Domain.Common;
using CodeWay.Domain.Exceptions;

/// <summary>Strongly-typed, validated email address value object.</summary>
public sealed class Email : BaseValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email() { }  // EF Core ctor

    private Email(string value) => Value = value;

    public static Email Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty.");

        if (!EmailRegex.IsMatch(value))
            throw new DomainException($"'{value}' is not a valid email address.");

        return new Email(value.ToLowerInvariant());
    }

    public static implicit operator string(Email email) => email.Value;
    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
