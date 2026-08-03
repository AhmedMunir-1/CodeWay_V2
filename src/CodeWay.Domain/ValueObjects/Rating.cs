namespace CodeWay.Domain.ValueObjects;

using CodeWay.Domain.Common;
using CodeWay.Domain.Exceptions;

/// <summary>
/// Rating value object constrained between 0.00 and 5.00.
/// Used on <c>InstructorProfile</c> and <c>Course</c> aggregates.
/// </summary>
public sealed class Rating : BaseValueObject
{
    public const decimal MinValue = 0.00m;
    public const decimal MaxValue = 5.00m;

    public decimal Value { get; }

    private Rating() { }  // EF Core ctor
    private Rating(decimal value) => Value = value;

    public static Rating Of(decimal value)
    {
        if (value < MinValue || value > MaxValue)
            throw new DomainException($"Rating must be between {MinValue} and {MaxValue}. Got: {value}");

        return new Rating(Math.Round(value, 2));
    }

    public static Rating Zero() => new(0m);

    public override string ToString() => Value.ToString("F2");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
